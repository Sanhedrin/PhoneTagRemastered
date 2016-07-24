using PhoneTag.WebServices;
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
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.SharedCodebase.Controllers;
using PhoneTag.WebServices.Utilities;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.POCOs;

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

            if (i_UserSocialView != null)
            {
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
            }
            else
            {
                success = false;
                ErrorLogger.Log("Invalid details given");
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

            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                User user = await GetUserModel(i_PlayerFBID);

                if (user != null)
                {
                    newReadyStatus = await user.SetReadyStatus(i_ReadyStatus);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }

            return newReadyStatus;
        }
        
        /// <summary>
        /// Pings activeness for the user, marking them as active.
        /// </summary>
        [Route("api/users/{i_PlayerFBID}/ping")]
        [HttpPost]
        public async Task<UserView> PingAsActive(String i_PlayerFBID)
        {
            User user = null;

            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                user = await GetUserModel(i_PlayerFBID);

                if (user != null)
                {
                    await user.PingAsActive();
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }

            return user == null ? null : await user.GenerateView();
        }

        /// <summary>
        /// Kills the given user.
        /// </summary>
        [Route("api/users/{i_PlayerFBID}/die")]
        [HttpPost]
        public async Task Kill(String i_PlayerFBID)
        {
            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                User user = await GetUserModel(i_PlayerFBID);

                if (user != null)
                {
                    if (!String.IsNullOrEmpty(user.PlayingIn))
                    {
                        GameRoom room = await RoomController.GetRoomModel(user.PlayingIn);

                        if (room != null)
                        {
                            await room.KillPlayer(i_PlayerFBID);
                        }
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }
        }

        /// <summary>
        /// Removes user from the current game.
        /// </summary>
        [Route("api/users/{i_PlayerFBID}/leave")]
        [HttpPost]
        public async Task LeaveGame(String i_PlayerFBID)
        {
            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                User user = await GetUserModel(i_PlayerFBID);

                if (user != null)
                {
                    if (!String.IsNullOrEmpty(user.PlayingIn))
                    {
                        GameRoom room = await RoomController.GetRoomModel(user.PlayingIn);

                        if(room != null)
                        {
                            await room.QuitRoom(i_PlayerFBID);
                        }
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }
        }

        /// <summary>
        /// Player's attempt at killing another player.
        /// </summary>
        [Route("api/users/{i_AttackerId}/kill/{i_AttackedId}/{i_KillCamId}")]
        [HttpPost]
        public async Task TryKill(String i_AttackerId, String i_AttackedId, String i_KillCamId)
        {
            if (!String.IsNullOrEmpty(i_AttackerId) && !String.IsNullOrEmpty(i_AttackedId) && !String.IsNullOrEmpty(i_KillCamId))
            {
                User attackedPlayer = await GetUserModel(i_AttackedId);

                if (attackedPlayer != null)
                {
                    await attackedPlayer.KillRequest(i_AttackerId, i_KillCamId);
                }
            }
            else
            {
                ErrorLogger.Log(String.Format("Invalid input values given: {0}, {1}, {2}", 
                    i_AttackedId, i_AttackerId, i_KillCamId));
            }
        }

        /// <summary>
        /// Update user current position.
        /// </summary>
        [Route("api/users/{i_FBID}/position")]
        [HttpPost]
        public async Task UpdateUserPosition([FromUri] String i_FBID, [FromBody] GeoPoint i_LocationInfo)
        {
            if (!String.IsNullOrEmpty(i_FBID) && i_LocationInfo != null)
            {
                User user = await GetUserModel(i_FBID);

                if (user != null)
                {
                    await user.UpdatePosition(i_LocationInfo.Latitude, i_LocationInfo.Longitude);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid details given");
            }
        }

        /// <summary>
        /// Sets the user as inactive.
        /// </summary>
        public static async Task Quit(ObjectId i_ExpiredId)
        {
            try
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", i_ExpiredId);

                BsonDocument userDoc = (await Mongo.Database.GetCollection<BsonDocument>("Users").FindAsync(filter)).First();

                await Quit(userDoc.GetValue("FBID").AsString);
            }
            catch(Exception e)
            {
                ErrorLogger.Log($"{e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }

        /// <summary>
        /// Sets the user as inactive.
        /// </summary>
        public static async Task Quit(string i_PlayerFBID)
        {
            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                User user = await GetUserModel(i_PlayerFBID);

                if (user != null)
                {
                    await user.Quit();
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }
        }

        /// <summary>
        /// Gets the model of the user whose id is given or null if such doesn't exist.
        /// </summary>
        public static async Task<User> GetUserModel(string i_FBID)
        {
            User foundUser = null;

            if (!String.IsNullOrEmpty(i_FBID))
            {
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
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }

            return foundUser;
        }
    }
}