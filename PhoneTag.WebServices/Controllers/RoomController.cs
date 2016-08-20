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
using PhoneTag.WebServices.Models;
using PhoneTag.SharedCodebase.Views;
using Nito.AsyncEx;
using PhoneTag.SharedCodebase.Utils;
using MongoDB.Driver.GeoJsonObjectModel;
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
using PhoneTag.WebServices.Controllers;
using PhoneTag.SharedCodebase.POCOs;

namespace PhoneTag.WebServices.Controllers
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
                    expiration.ExpirationTime = DateTime.Now.AddMinutes(60);
                    expiration._id = gameRoom._id;
                    await Mongo.Database.GetCollection<ExpirationEntry>("RoomExpiration").InsertOneAsync(expiration);
                }
                catch (Exception e)
                {
                    roomId = null;
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }

            return roomId;
        }

        /// <summary>
        /// Handles a kill dispute request.
        /// </summary>
        [Route("api/rooms/{i_RoomId}/dispute")]
        [HttpPost]
        public async Task HandleKillDispute([FromUri] string i_RoomId, [FromBody] KillDisputeEventArgs i_DisputeDetails)
        {
            if (!String.IsNullOrEmpty(i_RoomId) && i_DisputeDetails != null)
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if (room != null)
                {
                    room.HandleKillDispute(i_DisputeDetails);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid details given");
            }
        }

        /// <summary>
        /// Adds the player whose FBID is given to the given room.
        /// </summary>
        /// <returns>True or false based on if joining was successful</returns>
        [Route("api/rooms/{i_RoomId}/leave/{i_PlayerFBID}")]
        [HttpPost]
        public async Task LeaveRoom(string i_RoomId, string i_PlayerFBID)
        {
            if (!String.IsNullOrEmpty(i_RoomId) && !String.IsNullOrEmpty(i_PlayerFBID))
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if (room != null)
                {
                    room.LeaveRoom(i_PlayerFBID);
                }
            }
        }

        /// <summary>
        /// Kills the player from the current game.
        /// </summary>
        [Route("api/rooms/{i_RoomId}/kill/{i_PlayerFBID}")]
        [HttpPost]
        public async Task KillPlayer(string i_RoomId, string i_PlayerFBID)
        {
            if(!String.IsNullOrEmpty(i_RoomId) && !String.IsNullOrEmpty(i_PlayerFBID))
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                room.KillPlayer(i_PlayerFBID);
            }
            else
            {
                ErrorLogger.Log("Invalid input given");
            }
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

            if (!String.IsNullOrEmpty(i_RoomId) && !String.IsNullOrEmpty(i_PlayerFBID))
            {
                using (await sr_RoomChangeMutex.LockAsync())
                {
                    GameRoom room = await GetRoomModel(i_RoomId);

                    if (room != null)
                    {
                        success = await room.JoinGame(i_PlayerFBID);
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

        [Route("api/rooms/{i_RoomId}/targets/{i_FBID}/{i_Lat}/{i_Lng}/{i_Heading}")]
        [HttpGet]
        public async Task<List<UserView>> GetEnemiesInSight(String i_RoomId, String i_FBID, double i_Lat, double i_Lng, double i_Heading)
        {
            List<UserView> targets = new List<UserView>();

            if (!String.IsNullOrEmpty(i_RoomId) && !String.IsNullOrEmpty(i_FBID))
            {
                GeoPoint location = new GeoPoint(i_Lat, i_Lng);

                GameRoom room = await GetRoomModel(i_RoomId);

                if (room != null)
                {
                    //List<User> enemiesInSight = await room.GetEnemiesInSight(i_FBID, location, i_Heading);
                    IEnumerable<String> enemies = room.GameModeDetails.Mode.GetEnemiesFor(i_FBID).Union(room.LivingUsers);

                    foreach (String enemyId in enemies)
                    {
                        User enemy = await UsersController.GetUserModel(enemyId);

                        if (enemy != null /*&& !enemy.FBID.Equals(i_FBID)*/)
                        {
                            targets.Add(await enemy.GenerateView());
                        }
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Invalid room or user id");
            }

            return targets;
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

                FilterDefinition<GameRoom> filter = Builders<GameRoom>.Filter.Eq("Started", false);
                
                IFindFluent<GameRoom, GameRoom> gameRooms = Mongo.Database.GetCollection<GameRoom>("Rooms")
                    .Find(filter);

                if (gameRooms.Count() > 0)
                {
                    List<GameRoom> gameRoomList = await gameRooms.ToListAsync();

                    //Filter out all rooms out of range.
                    IEnumerable<GameRoom> matchingRooms = gameRoomList.Where(room => GeoUtils.GetDistanceBetween(
                        new GeoPoint(room.RoomLocation.Coordinates.Y, room.RoomLocation.Coordinates.X), 
                        location) < i_SearchRadius);

                    if(matchingRooms != null && matchingRooms.Count() > 0)
                    {
                        roomIds = matchingRooms.Where(
                            (room) => room.LivingUsers.Count < room.GameModeDetails.Mode.TotalNumberOfPlayers)
                            .Select(room => room._id.ToString()).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }

            return roomIds;
        }

        /// <summary>
        /// Gets in Game players locations.
        /// </summary>
        /// <param name="i_RoomId"></param>
        /// <returns>A dictionary of player id and a dynamic containing their name and location</returns>
        [Route("api/rooms/playersLocations/{i_RoomId}")]
        [HttpGet]
        public async Task<Dictionary<string, LocationUpdateInfo>> GetPlayersLocations(string i_RoomId)
        {
            Dictionary<string, LocationUpdateInfo> playersLocations = new Dictionary<string, LocationUpdateInfo>();

            if (!String.IsNullOrEmpty(i_RoomId))
            {
                GameRoom foundRoom = await GetRoomModel(i_RoomId);

                if (foundRoom != null)
                {
                    foreach (string userFBID in foundRoom.LivingUsers)
                    {
                        User player = await UsersController.GetUserModel(userFBID);

                        if (player?.CurrentLocation != null)
                        {
                            GeoPoint location = new GeoPoint(player.CurrentLocation.Coordinates.Y,
                                                             player.CurrentLocation.Coordinates.X);

                            LocationUpdateInfo playerInfo = new LocationUpdateInfo(player.FBID, player.Username, location);
                            playersLocations.Add(userFBID, playerInfo);
                        }
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Invalid RoomID given");
            }

            return playersLocations;
        }

        /// <summary>
        /// Gets in Game players locations.
        /// </summary>
        /// <param name="i_RoomId"></param>
        /// <returns>A dictionary of player id and his location</returns>
        [Route("api/rooms/{i_RoomId}/events/{i_CurrentEventId}")]
        [HttpGet]
        public async Task<List<Event>> GetEvents(string i_RoomId, int i_CurrentEventId)
        {
            List<Event> events = new List<Event>();

            if (!String.IsNullOrEmpty(i_RoomId))
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if(room != null)
                {
                    events = room.GetRoomEvents(i_CurrentEventId);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }

            return events;
        }

        /// <summary>
        /// Gets the model for the room of the given ID
        /// </summary>
        public static async Task<GameRoom> GetRoomModel(string i_RoomId)
        {
            GameRoom foundRoom = null;

            if (!String.IsNullOrEmpty(i_RoomId))
            {
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
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }
            else
            {
                ErrorLogger.Log("Invalid ID given");
            }

            return foundRoom;
        }
    }
}