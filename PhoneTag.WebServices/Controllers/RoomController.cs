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

namespace PhoneTag.WebServices.Controllers
{
    /// <summary>
    /// The controller to manage game room specific operations.
    /// </summary>
    public class RoomController : ApiController
    {
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
        /// Gets the room by the given id and returns a view of it.
        /// </summary>
        /// <param name="i_RoomId"></param>
        /// <returns></returns>
        [Route("api/rooms/{i_RoomId}")]
        [HttpGet]
        public async Task<GameRoomView> GetRoom(string i_RoomId)
        {
            GameRoom foundRoom = await getRoomModel(i_RoomId);

            return (foundRoom != null) ? foundRoom.GenerateView() : null;
        }

        private async Task<GameRoom> getRoomModel(string i_RoomId)
        {
            GameRoom foundRoom = null;

            try
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_Id", i_RoomId);

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