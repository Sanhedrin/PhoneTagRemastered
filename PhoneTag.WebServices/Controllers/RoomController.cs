using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using PhoneTag.SharedCodebase.Models;
using PhoneTag.SharedCodebase.Views;
using Nito.AsyncEx;
using PhoneTag.SharedCodebase.Utils;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System.Linq.Expressions;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Controllers;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.WebServices;
using PhoneTag.WebServices.Utilities;

namespace PhoneTag.SharedCodebase.Controllers
{
    /// <summary>
    /// The controller to manage game room specific operations.
    /// </summary>
    public class RoomController : ApiController
    {
        private static readonly AsyncLock sr_RoomChangeMutex = new AsyncLock();
        
        /// <summary>
        /// Creates a new game room.
        /// </summary>
        /// <param name="i_GameDetailsView">Details about the new game's rules</param>
        /// <returns>The ID of the created room.</returns>
        [Route("api/rooms/create")]
        [HttpPost]
        public async Task<String> CreateRoom(GameDetailsView i_GameDetailsView)
        {
            String roomId = null;

            if (i_GameDetailsView != null)
            {
                try
                {
                    GameRoom gameRoom = new GameRoom(GameDetails.FromView(i_GameDetailsView));
                    await Mongo.Database.GetCollection<GameRoom>("Rooms").InsertOneAsync(gameRoom);
                    roomId = gameRoom._id.ToString();

                    //Add the room to the expiration list.
                    ExpirationEntry expiration = new ExpirationEntry();
                    expiration.ExpirationTime = DateTime.Now.AddMinutes(1);
                    expiration._id = gameRoom._id;
                    await Mongo.Database.GetCollection<ExpirationEntry>("RoomExpiration").InsertOneAsync(expiration);
                }
                catch (Exception e)
                {
                    roomId = null;
                    ErrorLogger.Log(e.Message);
                }
            }

            return roomId;
        }

        /// <summary>
        /// Adds the player whose FBID is given to the given room.
        /// </summary>
        /// <returns>True or false based on if joining was successful</returns>
        [Route("api/rooms/{i_RoomId}/leave/{i_PlayerFBID}")]
        [HttpPost]
        public async Task LeaveRoom(string i_RoomId, string i_PlayerFBID)
        {
            if (i_RoomId != null && i_PlayerFBID != null)
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if (room != null)
                {
                    //We need to separate between the case a user leaves in the middle of a game or in the lobby
                    //Game case(If the player is already dead we don't need to doy anything)
                    if (room.Started && room.LivingUsers.Contains(i_PlayerFBID))
                    {
                        //In the case the user left in the middle of the game, we'll consider it as the player
                        //having died.
                        await KillPlayer(i_RoomId, i_PlayerFBID);
                    }
                    //If the game didn't yet start, we just remove the player
                    else if (!room.Started && room.LivingUsers.Contains(i_PlayerFBID))
                    {
                        try
                        {
                            room.LivingUsers.Remove(i_PlayerFBID);

                            //Update the room to add the player to it.
                            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(i_RoomId));
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                                .Set<List<String>>("LivingUsers", room.LivingUsers);

                            await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);
                        }
                        catch (Exception e)
                        {
                            ErrorLogger.Log(e.Message);
                        }

                        //Add the room as the user's current playing room.
                        await UsersController.LeaveRoom(i_PlayerFBID);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given room has all players ready and is ready to start the game.
        /// In which case a push notification is sent to all participating players to signal them to start.
        /// </summary>
        public static async Task CheckGameStart(string i_RoomId)
        {
            if (i_RoomId != null)
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if (room != null)
                {
                    bool readyToStart = true;

                    //Important to note:
                    //We allow players to start the game even if not enough players are present if everyone agrees to it.
                    foreach (String userId in room.LivingUsers)
                    {
                        User user = await UsersController.GetUserModel(userId);

                        if (user == null)
                        {
                            ErrorLogger.Log("Invalid User ID found in room's player list.");
                        }
                        else if (!user.IsReady)
                        {
                            readyToStart = false;
                            break;
                        }
                    }

                    //If all players are ready, start the game.
                    if (readyToStart)
                    {
                        startGame(room);
                    }
                }
            }
        }

        //Starts the game on the given room.
        private static async Task startGame(GameRoom i_Room)
        {
            if (i_Room != null)
            {
                try {
                    //Update the room to set it as started.
                    FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", i_Room._id);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("Started", true);

                    await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

                    update = Builders<BsonDocument>.Update.Set("ExpirationTime", DateTime.Now.AddMinutes(i_Room.GameModeDetails.GameDurationInMins));
                    await Mongo.Database.GetCollection<BsonDocument>("RoomExpiration").UpdateOneAsync(filter, update);

                    //Notify all players in the room that the game started.
                    String gameStartEventMessage = JsonConvert.SerializeObject(new GameStartEvent(i_Room._id.ToString()), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                    gameStartEventMessage = gameStartEventMessage.Replace('\"', '\'');

                    PushNotificationService pushService = App42API.BuildPushNotificationService();
                    pushService.SendPushMessageToGroup(gameStartEventMessage, i_Room.LivingUsers);
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.Message);
                }
            }
        }

        /// <summary>
        /// Kills the player from the current game.
        /// </summary>
        public async Task KillPlayer(string i_RoomId, string i_PlayerFBID)
        {
        }

        /// <summary>
        /// Adds the player whose FBID is given to the given room.
        /// </summary>
        /// <returns>True or false based on if joining was successful</returns>
        [Route("api/rooms/{i_RoomId}/join/{i_PlayerFBID}")]
        [HttpPost]
        public async Task<bool> JoinRoom(string i_RoomId, string i_PlayerFBID)
        {
            bool success = false;

            if (i_RoomId != null && i_PlayerFBID != null)
            {
                using (await sr_RoomChangeMutex.LockAsync())
                {
                    GameRoom room = await GetRoomModel(i_RoomId);

                    if (room != null && !room.Started && room.LivingUsers.Count < room.GameModeDetails.Mode.TotalNumberOfPlayers)
                    {
                        room.LivingUsers.Add(i_PlayerFBID);

                        try
                        {
                            //Update the room to add the player to it.
                            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(i_RoomId));
                            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                                .Set<List<String>>("LivingUsers", room.LivingUsers);

                            await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

                            //Add the room as the user's current playing room.
                            await UsersController.JoinRoom(i_PlayerFBID, i_RoomId);

                            success = true;
                        }
                        catch (Exception e)
                        {
                            success = false;
                            ErrorLogger.Log(e.Message);
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Gets the room by the given id and returns a view of it.
        /// </summary>
        /// <param name="i_RoomId"></param>
        /// <returns></returns>
        [Route("api/rooms/{i_RoomId}")]
        [HttpGet]
        public async Task<GameRoomView> GetRoom(string i_RoomId)
        {
            GameRoom foundRoom = await GetRoomModel(i_RoomId);

            return (foundRoom != null) ? await foundRoom.GenerateView() : null;
        }

        /// <summary>
        /// Searches all the existing pending rooms in nearby proximity to the user.
        /// </summary>
        /// <param name="i_Location">Location to use as the search base.</param>
        /// <param name="i_SearchRadius">Maximum distance of the play area from the user in km.</param>
        /// <returns>A list of matching room ids</returns>
        [Route("api/rooms/find/{i_Lat}/{i_Lng}/{i_SearchRadius}")]
        [HttpGet]
        public async Task<List<String>> GetRoomsInRange(double i_Lat, double i_Lng, float i_SearchRadius)
        {
            List<String> roomIds = new List<string>();

            try
            {
                GeoPoint location = new GeoPoint(i_Lat, i_Lng);

                FilterDefinition<GameRoom> filter = Builders<GameRoom>.Filter.And(
                        Builders<GameRoom>.Filter.Eq("Started", false),
                        Builders<GameRoom>.Filter.NearSphere(room => room.RoomLocation, GeoJson.Point<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(location.Longitude, location.Latitude)), i_SearchRadius));

                IFindFluent<GameRoom, String> gameModes = Mongo.Database.GetCollection<GameRoom>("Rooms")
                    .Find(filter)
                    .Project(room => room._id.ToString());

                if (gameModes.Count() > 0)
                {
                    roomIds = await gameModes.ToListAsync();
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.Message);
            }

            return roomIds;
        }

        /// <summary>
        /// Gets the model for the room of the given ID
        /// </summary>
        public static async Task<GameRoom> GetRoomModel(string i_RoomId)
        {
            GameRoom foundRoom = null;

            try
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(i_RoomId));

                IMongoCollection<BsonDocument> rooms = Mongo.Database.GetCollection<BsonDocument>("Rooms");

                if (await rooms.CountAsync(filter) > 0)
                {
                    using (IAsyncCursor<GameRoom> cursor = await rooms.FindAsync<GameRoom>(filter))
                    {
                        foundRoom = await cursor.SingleAsync();
                    }
                }
            }
            catch (Exception e)
            {
                foundRoom = null;
                ErrorLogger.Log(e.Message);
            }

            return foundRoom;
        }
    }
}