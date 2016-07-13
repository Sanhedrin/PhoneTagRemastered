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
using PhoneTag.WebServices.Models;
using Nito.AsyncEx;

namespace PhoneTag.WebServices.Controllers
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
            newUser.IsActive = true;
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
            using (await sr_UserChangeMutex.LockAsync())
            {
                //Add the room as the user's current playing room.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set("IsReady", i_ReadyStatus);

                await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
            }

            //TODO: Run check for game start
            return i_ReadyStatus;
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
        /// Sets the user as inactive.
        /// </summary>
        [Route("api/users/quit/{i_PlayerFBID}")]
        [HttpPost]
        public async Task Quit(string i_PlayerFBID)
        {
            using (await sr_UserChangeMutex.LockAsync())
            {
                //Add the room as the user's current playing room.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set("IsActive", false);

                await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
            }

            await LeaveRoom(i_PlayerFBID);
        }

        /// <summary>
        /// Sets the user as active.
        /// </summary>
        [Route("api/users/login/{i_PlayerFBID}")]
        [HttpPost]
        public async Task Login(string i_PlayerFBID)
        {
            using (await sr_UserChangeMutex.LockAsync())
            {
                //Add the room as the user's current playing room.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set("IsActive", true);

                await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
            }
        }

        /// <summary>
        /// Sets the given room as this player's active room.
        /// </summary>
        //[Route("api/users/{i_PlayerFBID}/join/{i_RoomId}")]
        //[HttpPost]
        public static async Task JoinRoom(string i_PlayerFBID, string i_RoomId)
        {
            using (await sr_UserChangeMutex.LockAsync())
            {
                //Add the room as the user's current playing room.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", i_PlayerFBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set<String>("PlayingIn", i_RoomId);

                await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
            }
        }

        /// <summary>
        /// Removes the player from the room they're currently at.
        /// </summary>
        public static async Task LeaveRoom(string i_PlayerFBID)
        {
            using (await sr_UserChangeMutex.LockAsync())
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
}