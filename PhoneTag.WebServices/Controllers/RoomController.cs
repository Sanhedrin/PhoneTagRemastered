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

            GameRoom gameRoom = new GameRoom(GameDetails.FromView(i_GameDetailsView));
            gameRoom.ExpirationTime = DateTime.Now;

            try
            {
                await Mongo.Database.GetCollection<GameRoom>("Rooms").InsertOneAsync(gameRoom);
                roomId = gameRoom._id.ToString();
            }
            catch (Exception e)
            {
                roomId = null;
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
            using (await sr_RoomChangeMutex.LockAsync())
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                //We need to separate between the case a user leaves in the middle of a game or in the lobby
                //Game case(If the player is already dead we don't need to doy anything)
                if (room.Started && room.LivingUsers.Contains(i_PlayerFBID))
                {
                    //In the case the user left in the middle of the game, we'll consider it as the player
                    //having died.
                    await KillPlayer(i_RoomId, i_PlayerFBID);
                }
                //If the game didn't yet start, we just remove the player
                else if (!room.Started && room.LivingUsers.Contains(i_PlayerFBID)) { 
                    room.LivingUsers.Remove(i_PlayerFBID);
                    
                    //Update the room to add the player to it.
                    FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(i_RoomId));
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                        .Set<List<String>>("LivingUsers", room.LivingUsers);

                    await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

                    //Add the room as the user's current playing room.
                    await UsersController.LeaveRoom(i_PlayerFBID);
                }
            }
        }

        /// <summary>
        /// Kills the player from the current game.
        /// </summary>
        public Task KillPlayer(string i_RoomId, string i_PlayerFBID)
        {
            throw new NotImplementedException();
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

            using (await sr_RoomChangeMutex.LockAsync())
            {
                GameRoom room = await GetRoomModel(i_RoomId);

                if (!room.Started && room.LivingUsers.Count < room.GameModeDetails.Mode.NumberOfPlayers)
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
        [Route("api/rooms/find/{i_Location}/{i_SearchRadius}")]
        [HttpGet]
        public async Task<List<String>> GetRoomsInRange(GeoPoint i_Location, float i_SearchRadius)
        {
            List<String> roomIds = new List<string>();

            IFindFluent<GameRoom, String> gameModes = Mongo.Database.GetCollection<GameRoom>("Rooms")
                .Find(Builders<GameRoom>.Filter.Empty)
                .Project(room => room._id.ToString());
            roomIds = await gameModes.ToListAsync();

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

                using (IAsyncCursor<GameRoom> cursor = await Mongo.Database.GetCollection<BsonDocument>("Rooms").FindAsync<GameRoom>(filter))
                {
                    foundRoom = await cursor.SingleAsync();
                }
            }
            catch (Exception e)
            {
                foundRoom = null;
            }

            return foundRoom;
        }
    }
}