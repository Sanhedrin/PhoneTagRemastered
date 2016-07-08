﻿using PhoneTag.SharedCodebase;
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

namespace PhoneTag.WebServices.Controllers
{
    /// <summary>
    /// The controller to manage user specific operations.
    /// </summary>
    public class UsersController : ApiController
    {
        /// <summary>
        /// Creates a new user with the given name.
        /// </summary>
        /// <param name="i_Username">A unique username.</param>
        /// <returns>Success indicator.</returns>
        [Route("api/users/create")]
        [HttpPost]
        public async Task<bool> CreateUser([FromBody]string i_Username)
        {
            bool success = true;

            User newUser = new User();

            newUser.Username = i_Username;
            newUser.Ammo = 3;
            newUser.IsReady = true;
            newUser.Friends = new List<User>();

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
        [Route("api/users/{i_Username}")]
        [HttpGet]
        public async Task<UserView> GetUser(string i_Username)
        {
            User foundUser = await getUserModel(i_Username);

            return (foundUser != null) ? foundUser.GenerateView() : null;
        }

        private async Task<User> getUserModel(string i_Username)
        {
            User foundUser = null;

            try
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Username", i_Username);

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
    }
}