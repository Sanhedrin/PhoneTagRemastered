using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;

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
        
        public List<User> LivingUsers { get { return new List<User>(r_LivingUsers); } }
        private readonly List<User> r_LivingUsers = new List<User>();
        
        public List<User> DeadUsers { get { return new List<User>(r_DeadUsers); } }
        private readonly List<User> r_DeadUsers = new List<User>();

        public GameRoom(GameDetails i_GameDetails)
        {
            GameModeDetails = i_GameDetails;
            Started = false;
            Finished = false;
            GameTime = 0;
        }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public dynamic GenerateView()
        {
            GameRoomView roomView = new GameRoomView();

            roomView.RoomId = _id.ToString();
            roomView.ExpirationTime = ExpirationTime;
            roomView.Finished = Finished;
            roomView.GameTime = GameTime;
            roomView.Started = Started;

            roomView.GameDetails = GameModeDetails.GenerateView();

            return roomView;
        }
    }
}
