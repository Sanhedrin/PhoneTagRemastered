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
    public class RoomController : ApiController
    {
        [Route("api/rooms/create")]
        [HttpPost]
        public async Task<bool> CreateRoom(GameDetailsView i_GameDetailsView)
        {
            bool success = true;

            GameRoom gameRoom = new GameRoom(GameDetails.FromView(i_GameDetailsView));

            try
            {
                await Mongo.Database.GetCollection<GameRoom>("Rooms").InsertOneAsync(gameRoom);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        [Route("api/rooms/{i_RoomId}")]
        [HttpGet]
        public async Task<User> GetRoom(string i_RoomId)
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