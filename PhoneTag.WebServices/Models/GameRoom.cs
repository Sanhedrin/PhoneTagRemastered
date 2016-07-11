using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;
using PhoneTag.WebServices.Controllers;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game room model.
    /// </summary>
    public class GameRoom : IViewable
    {
        public ObjectId _id { get; set; }

        public DateTime ExpirationTime { get; set; }

        public GameDetails GameModeDetails { get; set; }
        public bool Started { get; set; }
        public bool Finished { get; set; }
        public int GameTime { get; set; }
        
        public List<String> LivingUsers { get; private set; }
        
        public List<String> DeadUsers { get; private set; }

        public GameRoom(GameDetails i_GameDetails)
        {
            GameModeDetails = i_GameDetails;
            Started = false;
            Finished = false;
            GameTime = 0;
            LivingUsers = new List<string>();
            DeadUsers = new List<string>();
        }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            GameRoomView roomView = new GameRoomView();

            roomView.RoomId = _id.ToString();
            roomView.ExpirationTime = ExpirationTime;
            roomView.Finished = Finished;
            roomView.GameTime = GameTime;
            roomView.Started = Started;

            foreach (String userId in LivingUsers)
            {
                roomView.LivingUsers.Add(await (await UsersController.GetUserModel(userId)).GenerateView());
            }
            foreach (String userId in DeadUsers)
            {
                roomView.DeadUsers.Add(await (await UsersController.GetUserModel(userId)).GenerateView());
            }

            roomView.GameDetails = await GameModeDetails.GenerateView();

            return roomView;
        }
    }
}
