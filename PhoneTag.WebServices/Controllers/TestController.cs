using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.WebServices.Models;
using PhoneTag.WebServices;

namespace PhoneTag.SharedCodebase.Controllers
{
    /// <summary>
    /// Just some testing controls.
    /// </summary>
    public class TestController : ApiController
    {
        [Route("api/test/ping")]
        [HttpGet]
        public async Task<string> Ping()
        {
            string res = await ping();
            return "ping: " + res;
        }
        [Route("api/test/pong")]
        [HttpGet]
        public async Task<string> Pong()
        {
            string res = await pong();
            return "pong: " + res;
        }

        private async Task<string> ping()
        {
            try {
                IMongoCollection<BsonDocument> col = Mongo.Database.GetCollection<BsonDocument>("myCollection");
                CreateIndexOptions creationOptions = new CreateIndexOptions();
                creationOptions.ExpireAfter = new TimeSpan(TimeSpan.TicksPerSecond * 5);
                IndexKeysDefinition<BsonDocument> keys = Builders<BsonDocument>.IndexKeys.Ascending("time");
                await col.Indexes.DropAllAsync();
                await col.Indexes.CreateOneAsync(keys, creationOptions);
                for (int i = 0; i < 10; ++i)
                {
                    col.InsertOne(new BsonDocument { { "time", DateTime.Now.AddSeconds(i) }, { "User", new User() { Username = "user" + i.ToString() }.ToBsonDocument() } });
                }
            }
            catch(Exception e)
            {
                return "error is: " + e.Message;
            }

            return "";
        }

        //[Route("api/init")]
        //[HttpGet]
        //public string Init()
        //{
        //    return Mongo.Init();
        //}

        private async Task<string> pong()
        {
            IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>("myCollection");
            
            string message = "";

            try {
                BsonDocument filter = new BsonDocument();

                using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter))
                {
                    await cursor.ForEachAsync(document =>
                    {
                        // process document
                        message += document.ToJson() + Environment.NewLine;
                    });
                }
            }
            catch(Exception e)
            {
                message = "The error is: " + e.Message;
            }

            return message;
        }

        [Route("api/test/clear")]
        [HttpGet]
        public async Task<string> ClearPositions()
        {
            await Mongo.Database.GetCollection<BsonDocument>("Users").DeleteManyAsync(new BsonDocument());
            await Mongo.Database.GetCollection<BsonDocument>("Test").DeleteManyAsync(new BsonDocument());
            await Mongo.Database.GetCollection<BsonDocument>("myCollection").DeleteManyAsync(new BsonDocument());
            return "cleared";
        }

        //[Route("api/test/position/{i_PlayerId}")]
        //[HttpPost]
        //public Point PositionUpdate([FromBody]Point i_PlayerLocation, int i_PlayerId)
        //{
        //    Redis.Database.GeoAdd("Test", new GeoLocation { Name = String.Format("{0}", i_PlayerId), Longitude = i_PlayerLocation.X, Latitude = i_PlayerLocation.Y });

        //    return i_PlayerLocation;
        //}

        //[Route("api/test/shoot/{i_PlayerId}")]
        //[HttpPost]
        //public String Shoot([FromBody]DeviceLocationInfo i_PlayerLocation, int i_PlayerId)
        //{
        //    List<string> hits = Redis.Database.GeoRadius("Test", i_PlayerLocation.DeviceLocation.X, i_PlayerLocation.DeviceLocation.Y, 20000000).ToList();

        //    hits = hits.Where((hitName) => !hitName.Equals(i_PlayerId.ToString())).ToList();

        //    return hits.Count > 0 ? hits[0] : "No hits";
        //}
    }
}