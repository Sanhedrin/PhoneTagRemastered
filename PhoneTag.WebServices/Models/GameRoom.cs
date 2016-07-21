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
