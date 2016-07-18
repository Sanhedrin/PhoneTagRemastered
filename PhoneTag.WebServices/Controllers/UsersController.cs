using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.IO;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Models;
using Nito.AsyncEx;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.Events.OpLogEvents;

namespace PhoneTag.SharedCodebase.Controllers
{
    /// <summary>
    /// The controller to manage user specific operations.
    /// </summary>
    public class UsersController : ApiController
    {
        private static readonly AsyncLock sr_UserChangeMutex = new AsyncLock();
        
        /// <summary>
        /// Creates a new user with the given name.
        /// </summary>
        /// <param name="i_Username">A unique username.</param>
        /// <returns>Success indicator.</returns>
        [Route("api/users/create")]
        [HttpPost]
        public async Task<bool> CreateUser([FromBody]UserSocialView i_UserSocialView)
        {
            bool success = true;

            User newUser = new User();

            newUser.FBID = i_UserSocialView.Id;
            newUser.Username = i_UserSocialView.Name;
            newUser.ProfilePicUrl = i_UserSocialView.ProfilePictureUrl;
            newUser.Ammo = 3;
            newUser.IsReady = false;
            newUser.IsActive = false;
            newUser.Friends = new List<String>();

            try
            {
                await Mongo.Database.GetCollection<User>("Users").InsertOneAsync(newUser);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }
        
        /// <summary>
        /// Gets a view of the user with the given username.
        /// </summary>
        [Route("api/users/{i_FBID}")]
        [HttpGet]
        public async Task<UserView> GetUser(string i_FBID)
        {
            User foundUser = await GetUserModel(i_FBID);

            return (foundUser != null) ? (await foundUser.GenerateView()) : null;
        }

        /// <summary>
        /// Sets the player's ready status
        /// </summary>
        [Route("api/users/ready/{i_PlayerFBID}")]
        [HttpPost]
        public async Task<bool> SetReadyStatus(String i_PlayerFBID, [FromBody]bool i_ReadyStatus)
        {
            bool newReadyStatus = i_ReadyStatus;

            //Add the room as the user's current playing room.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsReady", i_ReadyStatus);

            IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("Users");

            //Checks if the game should start.
            BsonDocument room = (await users.FindAsync(filter)).First();

            String roomId = room.GetValue("PlayingIn").IsBsonNull ? null : room.GetValue("PlayingIn").AsString;

            if (String.IsNullOrEmpty(roomId))
            {
                newReadyStatus = false;
            }
            else if (newReadyStatus)
            {
                await users.UpdateOneAsync(filter, update);
                RoomController.CheckGameStart(roomId);
            }

            return newReadyStatus;
        }

        /// <summary>
        /// Gets the model of the user whose id is given or null if such doesn't exist.
        /// </summary>
        public static async Task<User> GetUserModel(string i_FBID)
        {
            User foundUser = null;

            try
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_FBID);

                using (IAsyncCursor<User> cursor = await Mongo.Database.GetCollection<BsonDocument>("Users").FindAsync<User>(filter))
                {
                    foundUser = await cursor.SingleAsync();
                }
            }
            catch (Exception e)
            {
                foundUser = null;
            }

            return foundUser;
        }
        
        /// <summary>
        /// Pings activeness for the user, marking them as active.
        /// </summary>
        [Route("api/users/{i_PlayerFBID}/ping")]
        [HttpPost]
        public async Task<UserView> PingAsActive(String i_PlayerFBID)
        {
            //Set the user's activity state.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsActive", true);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);

            User user = await GetUserModel(i_PlayerFBID);

            //Refresh the expiration on activity
            BsonDocument expiration = new BsonDocument()
            {
                { "_id", user._id },
                {  "ExpirationTime", DateTime.Now.AddSeconds(60) }
            };

            FilterDefinition<BsonDocument> expFilter = Builders<BsonDocument>.Filter.Eq("_id", user._id);
            await Mongo.Database.GetCollection<BsonDocument>("UserExpiration")
                .ReplaceOneAsync(expFilter, expiration, new UpdateOptions { IsUpsert = true });

            //At a later point, if we'll choose we want to, we can use the returned value from this replacement
            //operation to tell if it was an insert or an update.
            //This helps in determining whether the user just logged on or if we're just pinging.
            //As a result, we can use that info to update the user's friends about them logging on.

            return await user.GenerateView();
        }

        /// <summary>
        /// Sets the user as inactive.
        /// </summary>
        public static async Task Quit(ObjectId i_ExpiredId)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", i_ExpiredId);

            BsonDocument userDoc = (await Mongo.Database.GetCollection<BsonDocument>("Users").FindAsync(filter)).First();

            await Quit(userDoc.GetValue("FBID").AsString);
        }

        /// <summary>
        /// Sets the user as inactive.
        /// </summary>
        public static async Task Quit(string i_PlayerFBID)
        {
            //Set the user's activity state.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsActive", false);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);

            User user = await GetUserModel(i_PlayerFBID);

            if (!String.IsNullOrEmpty(user.PlayingIn))
            {
                await new RoomController().LeaveRoom(user.PlayingIn, i_PlayerFBID);
            }
        }

        /// <summary>
        /// Sets the given room as this player's active room.
        /// </summary>
        //[Route("api/users/{i_PlayerFBID}/join/{i_RoomId}")]
        //[HttpPost]
        public static async Task JoinRoom(string i_PlayerFBID, string i_RoomId)
        {
            //Add the room as the user's current playing room.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set<String>("PlayingIn", i_RoomId);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Removes the player from the room they're currently at.
        /// </summary>
        public static async Task LeaveRoom(string i_PlayerFBID)
        {
            //Add the room as the user's current playing room.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set<String>("PlayingIn", null)
                .Set("IsReady", false);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
        }
    }
}