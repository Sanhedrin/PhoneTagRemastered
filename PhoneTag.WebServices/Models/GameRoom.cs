using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;
using PhoneTag.SharedCodebase.Controllers;
using MongoDB.Driver.GeoJsonObjectModel;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.WebServices.Controllers;
using MongoDB.Driver;
using PhoneTag.WebServices.Utilities;
using PhoneTag.SharedCodebase.Events.GameEvents;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game room model.
    /// </summary>
    public class GameRoom : IViewable
    {
        public ObjectId _id { get; private set; }

        public GameDetails GameModeDetails { get; private set; }
        public bool Started { get; private set; }
        public bool Finished { get; private set; }
        public int GameTime { get; private set; }
        public GeoJsonPoint<GeoJson2DCoordinates> RoomLocation { get; private set; }

        public List<String> LivingUsers { get; private set; }
        
        public List<String> DeadUsers { get; private set; }

        public GameRoom(GameDetails i_GameDetails)
        {
            GameModeDetails = i_GameDetails;
            RoomLocation = i_GameDetails.StartLocation;
            Started = false;
            Finished = false;
            GameTime = 0;
            LivingUsers = new List<string>();
            DeadUsers = new List<string>();
        }

        /// <summary>
        /// Expires the room, closing it.
        /// If the room is an ongoing game, this signals the end of the game.
        /// </summary>
        public async Task Expire()
        {
            if (!this.Started)
            {
                closePendingGame();
            }
            else
            {
                closeOngoingGame();
            }
        }

        //Closes an ongoing game room.
        private async Task closeOngoingGame()
        {
            closePendingGame();
        }

        //Closes a pending game room.
        private async Task closePendingGame()
        {
            foreach (String userId in this.LivingUsers)
            {
                User user = await UsersController.GetUserModel(userId);

                if (user != null)
                {
                    await LeaveRoom(userId);
                }
            }

            FilterDefinition<BsonDocument> roomFilter = Builders<BsonDocument>.Filter.Eq("_id", _id);
            await Mongo.Database.GetCollection<BsonDocument>("Rooms").DeleteOneAsync(roomFilter);
        }

        /// <summary>
        /// Removes the given player from this room.
        /// </summary>
        public async Task LeaveRoom(string i_PlayerFBID)
        {
            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                //We need to separate between the case a user leaves in the middle of a game or in the lobby
                //Game case(If the player is already dead we don't need to doy anything)
                if (this.Started && this.LivingUsers.Contains(i_PlayerFBID))
                {
                    //In the case the user left in the middle of the game, we'll consider it as the player
                    //having died.
                    await KillPlayer(i_PlayerFBID);
                }
                //If the game didn't yet start, we just remove the player
                else if (!this.Started && this.LivingUsers.Contains(i_PlayerFBID))
                {
                    try
                    {
                        this.LivingUsers.Remove(i_PlayerFBID);

                        //Update the room to add the player to it.
                        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                            .Set<List<String>>("LivingUsers", this.LivingUsers);

                        await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                    }

                    //Add the room as the user's current playing room.
                    User user = await UsersController.GetUserModel(i_PlayerFBID);
                    await user.LeaveRoom();
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }
        }

        /// <summary>
        /// Kills the given player removing them from the room.
        /// </summary>
        /// <param name="i_PlayerFBID"></param>
        /// <returns></returns>
        public async Task KillPlayer(string i_PlayerFBID)
        {
        }

        /// <summary>
        /// Adds the given player to this room.
        /// </summary>
        /// <param name="i_PlayerFBID"></param>
        public async Task<bool> JoinGame(string i_PlayerFBID)
        {
            bool success = false;

            if (!String.IsNullOrEmpty(i_PlayerFBID))
            {
                if (!this.Started && this.LivingUsers.Count < this.GameModeDetails.Mode.TotalNumberOfPlayers)
                {
                    this.LivingUsers.Add(i_PlayerFBID);

                    try
                    {
                        await removeUserFromAllRooms(i_PlayerFBID);

                        //Update the room to add the player to it.
                        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
                        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                            .Set<List<String>>("LivingUsers", this.LivingUsers);

                        await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

                        //Add the room as the user's current playing room.
                        User user = await UsersController.GetUserModel(i_PlayerFBID);

                        if (user != null)
                        {
                            await user.JoinRoom(this._id.ToString());

                            //Notify all players in the room that a player joined the room started.
                            PushNotificationUtils.PushEvent(new JoinRoomEvent(this._id.ToString()), this.LivingUsers);

                            success = true;
                        }
                    }
                    catch (Exception e)
                    {
                        success = false;
                        ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Invalid FBID given");
            }

            return success;
        }

        //Normally, we can track a user's quitting and remove them from the room they're currently listed in.
        //However, detecting the quit takes up to a minute.
        //If the user closes the application, logs in within the minute and then joins another room, a ghost
        //of that user will be left in the last room.
        //To fix that, we'll remove the player from any room they're listed in when joining a room.
        //Along with the quit detection, this takes care of all ghosting situations.
        private async Task removeUserFromAllRooms(String i_FBID)
        {
            try
            {
                //Update the room to add the player to it.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter
                    .AnyEq("LivingUsers", i_FBID);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Pull("LivingUsers", i_FBID);

                await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }
        }

        /// <summary>
        /// Checks if the given room has all players ready and is ready to start the game.
        /// In which case a push notification is sent to all participating players to signal them to start.
        /// </summary>
        public async Task CheckGameStart()
        {
            bool readyToStart = true;

            //Important to note:
            //We allow players to start the game even if not enough players are present if everyone agrees to it.
            foreach (String userId in this.LivingUsers)
            {
                User user = await UsersController.GetUserModel(userId);

                if (user == null)
                {
                    ErrorLogger.Log("Invalid User ID found in room's player list.");
                }
                else if (!user.IsReady)
                {
                    readyToStart = false;
                    break;
                }
            }

            //If all players are ready, start the game.
            if (readyToStart)
            {
                startGame();
            }
        }

        //Starts the game on the given room.
        private async Task startGame()
        {
            try
            {
                await setTeams();

                //Update the room to set it as started.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", this._id);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("Started", true);

                await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

                update = Builders<BsonDocument>.Update.Set("ExpirationTime", DateTime.Now.AddMinutes(this.GameModeDetails.GameDurationInMins));
                await Mongo.Database.GetCollection<BsonDocument>("RoomExpiration").UpdateOneAsync(filter, update);

                //Notify all players in the room that the game started.
                PushNotificationUtils.PushEvent(new GameStartEvent(this._id.ToString()), this.LivingUsers);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }
        }

        /// <summary>
        /// Gets a list of all enemies that are considered to be in my current sight, for suggestion purposes.
        /// </summary>
        public async Task<List<User>> GetEnemiesInSight(string i_FBID, GeoPoint i_Location, double i_Heading)
        {
            List<User> targets = new List<User>();

            try
            {
                List<String> allTargetIds = this.GameModeDetails.Mode.GetEnemiesFor(i_FBID);
                                
                if (allTargetIds != null && allTargetIds.Count > 0)
                {
                    //We only care about the living enemies.
                    IEnumerable<String> livingTargetIds = allTargetIds.Intersect(LivingUsers);

                    foreach (String userId in livingTargetIds)
                    {
                        User user = await UsersController.GetUserModel(userId);
                    }
                }
            }
            catch(Exception e)
            {
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }

            return targets;
        }

        //When the game starts, we'll randomize the players into teams according to the game's rules.
        private async Task setTeams()
        {
            GameModeDetails.Mode.ArrangeTeams(LivingUsers);

            //Update the newly created teams in the database.
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", this._id);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("GameModeDetails", GameModeDetails);

            await Mongo.Database.GetCollection<BsonDocument>("Rooms").UpdateOneAsync(filter, update);

        }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            GameRoomView roomView = new GameRoomView();

            roomView.RoomId = _id.ToString();
            roomView.Finished = Finished;
            roomView.GameTime = GameTime;
            roomView.Started = Started;

            if (RoomLocation != null && RoomLocation.Coordinates != null)
            {
                roomView.RoomLocation = new GeoPoint(RoomLocation.Coordinates.X, RoomLocation.Coordinates.Y);
            }

            foreach (String userId in LivingUsers)
            {
                User user = await UsersController.GetUserModel(userId);

                if (user != null)
                {
                    roomView.LivingUsers.Add(await user.GenerateView());
                }
            }
            foreach (String userId in DeadUsers)
            {
                User user = await UsersController.GetUserModel(userId);

                if (user != null)
                {
                    roomView.DeadUsers.Add(await user.GenerateView());
                }
            }

            if (GameModeDetails != null)
            {
                roomView.GameDetails = await GameModeDetails.GenerateView();
            }

            return roomView;
        }
    }
}
