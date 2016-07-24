using PhoneTag.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;
using System.Threading.Tasks;
using PhoneTag.WebServices.Controllers;
using MongoDB.Driver.GeoJsonObjectModel;
using PhoneTag.SharedCodebase.Utils;
using MongoDB.Driver;
using PhoneTag.WebServices.Utilities;
using PhoneTag.SharedCodebase.Events.GameEvents;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The user model.
    /// </summary>
    public class User : IViewable
    {
        public ObjectId _id { get; set; }
        public String FBID { get; set; }
        public String Username { get; set; }
        public String ProfilePicUrl { get; set; }
        public List<String> Friends { get; set; }
        public bool IsReady { get; set; }
        public bool IsActive { get; set; }
        public int Ammo { get; set; }
        public String PlayingIn { get; set; }
        public GeoJsonPoint<GeoJson2DCoordinates> CurrentLocation { get; set; }

        /// <summary>
        /// Sets the ready status of the current player.
        /// </summary>
        public async Task<bool> SetReadyStatus(bool i_ReadyStatus)
        {
            bool newReadyStatus = i_ReadyStatus;
            
            //Add the room as the user's current playing room.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsReady", i_ReadyStatus);

            IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("Users");

            await users.UpdateOneAsync(filter, update);
            
            //Checks if the game should start.
            if (String.IsNullOrEmpty(PlayingIn))
            {
                newReadyStatus = false;
            }
            else 
            {
                GameRoom room = await RoomController.GetRoomModel(PlayingIn);

                if (room != null && room.LivingUsers.Count > 0)
                {
                    PushNotificationUtils.PushEvent(new GameLobbyUpdateEvent(PlayingIn), room.LivingUsers);
                }

                if (newReadyStatus && room != null)
                {
                    room.CheckGameStart();
                }
            }

            return newReadyStatus;
        }

        /// <summary>
        /// Marks the player as active and updates their details.
        /// </summary>
        /// <returns></returns>
        public async Task PingAsActive()
        {
            //Set the user's activity state.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsActive", true);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);


            //Refresh the expiration on activity
            BsonDocument expiration = new BsonDocument()
            {
                { "_id", this._id },
                {  "ExpirationTime", DateTime.Now.AddSeconds(60) }
            };

            FilterDefinition<BsonDocument> expFilter = Builders<BsonDocument>.Filter.Eq("_id", this._id);
            await Mongo.Database.GetCollection<BsonDocument>("UserExpiration")
                .ReplaceOneAsync(expFilter, expiration, new UpdateOptions { IsUpsert = true });

            //At a later point, if we'll choose we want to, we can use the returned value from this replacement
            //operation to tell if it was an insert or an update.
            //This helps in determining whether the user just logged on or if we're just pinging.
            //As a result, we can use that info to update the user's friends about them logging on.
        }

        /// <summary>
        /// Disconnects the user from the game server marking them as inactive.
        /// </summary>
        /// <returns></returns>
        public async Task Quit()
        {
            //Set the user's activity state.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("IsActive", false);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);

            if (!String.IsNullOrEmpty(this.PlayingIn))
            {
                GameRoom room = await RoomController.GetRoomModel(PlayingIn);

                if (room != null)
                {
                    await room.LeaveRoom(FBID);
                }
            }
        }

        /// <summary>
        /// Sends a kill request to this player by the given player, which should be displayed to the
        /// killed player.
        /// </summary>
        public async Task KillRequest(string i_RequestedByFBID, String i_KillCamId)
        {
            User killer = await UsersController.GetUserModel(i_RequestedByFBID);

            if (killer != null)
            {
                try
                {
                    KillRequestEvent killRequestEvent = new KillRequestEvent(PlayingIn, killer.Username, i_KillCamId);
                    PushNotificationUtils.PushEvent(killRequestEvent, new List<string>() { FBID });
                }
                catch(Exception e)
                {
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }
        }

        /// <summary>
        /// Sets the given room as this player's active room.
        /// </summary>
        public async Task JoinRoom(string i_RoomId)
        {
            if (!String.IsNullOrEmpty(i_RoomId))
            {
                //Add the room as the user's current playing room.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set("PlayingIn", i_RoomId);

                await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);

                await SetReadyStatus(false);
            }
            else
            {
                ErrorLogger.Log("Invalid room given");
            }
        }

        /// <summary>
        /// Removes the player from the room they're currently at.
        /// </summary>
        public async Task LeaveRoom()
        {
            //Add the room as the user's current playing room.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set<String>("PlayingIn", null)
                .Set("IsReady", false);

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
        }

        public async Task UpdatePosition(double i_Latitude, double i_Longitude)
        {
            //update current user position. 
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("FBID", FBID);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("CurrentLocation", new GeoJsonPoint<GeoJson2DCoordinates>(new GeoJson2DCoordinates(i_Longitude, i_Latitude)));

            await Mongo.Database.GetCollection<BsonDocument>("Users").UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Generates a view for this model,
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            UserView userView = new UserView();

            userView.FBID = FBID;
            userView.Username = Username;
            userView.ProfilePicUrl = ProfilePicUrl;
            userView.IsReady = IsReady;
            userView.IsActive = IsActive;
            userView.Ammo = Ammo;

            if (CurrentLocation != null)
            {
                userView.CurrentLocation = new GeoPoint(CurrentLocation.Coordinates.Y, CurrentLocation.Coordinates.X);
            }

            if (PlayingIn != null)
            {
                userView.PlayingIn = PlayingIn;
            }

            //We can't start generating views for each of my friends because it'll cause a cyclic
            //infinite loop.
            //We might not care about the entirety of the list though, and only get basic details.
            //If more information is required, the friend's username can be used to poll it.
            userView.Friends = new List<UserView>();
            foreach (String friendId in Friends)
            {
                UserView friendView = new UserView();
                User friend = await UsersController.GetUserModel(friendId);

                friendView.Username = friend.Username;
                friendView.FBID = friend.FBID;
                friendView.ProfilePicUrl = friend.ProfilePicUrl;
                friendView.Ammo = friend.Ammo;
                friendView.IsReady = friend.IsReady;
                friendView.Friends = null;
                friendView.IsActive = friend.IsActive;

                if (friend.CurrentLocation != null)
                {
                    friendView.CurrentLocation = new GeoPoint(friend.CurrentLocation.Coordinates.Y, friend.CurrentLocation.Coordinates.X);
                }

                userView.Friends.Add(friendView);
            }

            return userView;
        }
    }
}