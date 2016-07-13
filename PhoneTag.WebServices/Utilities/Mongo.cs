using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.WebServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.WebServices
{
    /// <summary>
    /// Allows access to our database.
    /// </summary>
    public class Mongo
    {
        private const long k_SecondsPerHour = 60*60;

        private static IMongoClient s_Client;
        public static IMongoDatabase Database { get; private set; }
        public static bool IsReady { get; private set; }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        public static string Init()
        {
            String errorMessage = "All's well";

            try
            {
                //s_Client = new MongoClient("mongodb://Sanhedrin123:Sanhedrin123@ds040309.mlab.com:40309/ptdb");
                s_Client = new MongoClient();
                Database = s_Client.GetDatabase("ptdb");

                IsReady = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            rebuildIndexes();

            return errorMessage;
        }

        private static async Task rebuildIndexes()
        {
            //If the database is out of date, rebuild the indexes.
            if (!(await Mongo.Database.GetCollection<BsonDocument>("FloatingValues").FindAsync(Builders<BsonDocument>.Filter.Eq("Ready", "true"))).Any())
            {
                IMongoCollection<GameRoom> col = Mongo.Database.GetCollection<GameRoom>("Rooms");

                await Database.GetCollection<User>("Users").Indexes.DropAllAsync();
                await col.Indexes.DropAllAsync();

                //Create timed index for rooms
                CreateIndexOptions creationOptions = new CreateIndexOptions();
                creationOptions.ExpireAfter = new TimeSpan(TimeSpan.TicksPerSecond * k_SecondsPerHour);
                IndexKeysDefinition<GameRoom> keys = Builders<GameRoom>.IndexKeys.Ascending("ExpirationTime");
                await col.Indexes.CreateOneAsync(keys, creationOptions);

                //Create username index for users.
                await Database.GetCollection<User>("Users").Indexes.CreateOneAsync(
                    Builders<User>.IndexKeys.Ascending("FBID"),
                    new CreateIndexOptions<User>() { Unique = true }
                );

                //Create a geographic index for our games.
                await Database.GetCollection<GameRoom>("Rooms").Indexes.CreateOneAsync(
                    Builders<GameRoom>.IndexKeys.Geo2DSphere(room => room.RoomLocation)
                );

                //Create a geographic index for our users
                await Database.GetCollection<User>("Users").Indexes.CreateOneAsync(
                    Builders<User>.IndexKeys.Geo2DSphere(x => x.CurrentLocation)
                );

                Database.GetCollection<BsonDocument>("FloatingValues").InsertOne(new BsonDocument { { "Ready", "true" } });
            }
        }
    }
}