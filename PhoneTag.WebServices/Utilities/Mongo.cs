using com.shephertz.app42.paas.sdk.csharp;
using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PhoneTag.SharedCodebase.Events.OpLogEvents;

namespace PhoneTag.SharedCodebase
{
    /// <summary>
    /// Allows access to our database.
    /// </summary>
    public static class Mongo
    {
        private const long k_SecondsPerHour = 60*60;
        private const long k_SecondsPerMinute = 60;
        
        private const string k_DBName = "ptdb";
        private const string k_LocalDBName = "local";

        private static IMongoClient s_Client;
        public static IMongoDatabase Database { get; private set; }
        public static IMongoDatabase LocalDatabase { get; private set; }
        public static bool IsReady { get; private set; }

        private const String k_ServerVersion = "1.0.0.0";

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

                Database = s_Client.GetDatabase(k_DBName);
                LocalDatabase = s_Client.GetDatabase(k_LocalDBName);

                IsReady = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            
            updateVersion();
            rebuildIndexes();

            return errorMessage;
        }

        //If installing a fresh copy or uploading a version of the server that's newer than the version
        //stored in the database, update the database's saved version.
        private static async Task updateVersion()
        {
            FilterDefinition<BsonDocument> serverInfoFilter = Builders<BsonDocument>.Filter.Eq("Type", "ServerInfo");
            UpdateDefinition<BsonDocument> updateVersion = Builders<BsonDocument>.Update.Set("Version", k_ServerVersion);
            
            //If the ServerInfo document doesn't exist, we'll create it.
            if ((await Database.GetCollection<BsonDocument>("Info").CountAsync(Builders<BsonDocument>.Filter.Empty)) == 0)
            {
                await Database.GetCollection<BsonDocument>("Info")
                    .InsertOneAsync(new BsonDocument() {
                        { "Type", "ServerInfo" },
                        { "Version", k_ServerVersion }
                    });
            }
            else
            {
                BsonDocument info = await Database.GetCollection<BsonDocument>("Info").Find(serverInfoFilter).FirstAsync();

                //Otherwise, if the version is out of date, update it.
                if (!info.GetValue("Version").AsString.Equals(k_ServerVersion))
                {
                    await Database.GetCollection<BsonDocument>("Info")
                        .UpdateOneAsync(serverInfoFilter, updateVersion);
                }
            }
        }

        private static async Task rebuildIndexes()
        {
            //If the database is out of date, rebuild the indexes.
            if (!(await Mongo.Database.GetCollection<BsonDocument>("FloatingValues").FindAsync(Builders<BsonDocument>.Filter.Eq("Ready", "true"))).Any())
            {
                await Database.GetCollection<ExpirationEntry>("RoomExpiration").Indexes.DropAllAsync();
                await Database.GetCollection<ExpirationEntry>("UserExpiration").Indexes.DropAllAsync();
                await Database.GetCollection<User>("Users").Indexes.DropAllAsync();
                await Database.GetCollection<GameRoom>("Rooms").Indexes.DropAllAsync();

                //Create timed index for rooms
                CreateIndexOptions creationOptions = new CreateIndexOptions();
                IndexKeysDefinition<ExpirationEntry> keys = Builders<ExpirationEntry>.IndexKeys.Ascending("ExpirationTime");
                creationOptions.ExpireAfter = new TimeSpan(0);
                //Adding expiration indexes for rooms.
                await Database.GetCollection<ExpirationEntry>("RoomExpiration").Indexes.CreateOneAsync(keys, creationOptions);
                //Adding expiration indexes for users.
                await Database.GetCollection<ExpirationEntry>("UserExpiration").Indexes.CreateOneAsync(keys, creationOptions);

                //Create username index for users.
                await Database.GetCollection<User>("Users").Indexes.CreateOneAsync(
                    Builders<User>.IndexKeys.Ascending("FBID"),
                    new CreateIndexOptions<User>() { Unique = true }
                );
                //Create a geographic index for our users
                await Database.GetCollection<User>("Users").Indexes.CreateOneAsync(
                    Builders<User>.IndexKeys.Geo2DSphere(x => x.CurrentLocation)
                );

                //Create a geographic index for our games.
                await Database.GetCollection<GameRoom>("Rooms").Indexes.CreateOneAsync(
                    Builders<GameRoom>.IndexKeys.Geo2DSphere(room => room.RoomLocation)
                );

                Database.GetCollection<BsonDocument>("FloatingValues").InsertOne(new BsonDocument { { "Ready", "true" } });
            }
        }
    }
}