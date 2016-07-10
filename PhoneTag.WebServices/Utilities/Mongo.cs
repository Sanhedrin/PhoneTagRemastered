using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.WebServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async static void rebuildIndexes()
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
                Database.GetCollection<User>("Users").Indexes.CreateOne(
                    Builders<User>.IndexKeys.Ascending("FBID"),
                    new CreateIndexOptions<User>() { Unique = true }
                );

                Database.GetCollection<BsonDocument>("FloatingValues").InsertOne(new BsonDocument { { "Ready", "true" } });
            }
        }
    }
}