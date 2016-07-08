using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    /// <summary>
    /// A view representing a game room, allows interaction with the server on per room basis.
    /// </summary>
    public class GameRoomView : IUpdateable
    {
        public String RoomId { get; set; }
        public GameDetailsView GameModeDetails { get; set; }
        public bool Started { get; set; }
        public bool Finished { get; set; }
        public int GameTime { get; set; }
        public List<UserView> LivingUsers { get; set; }
        public List<UserView> DeadUsers { get; set; }

        public GameRoomView()
        {
        }

        /// <summary>
        /// Creates a new game room.
        /// </summary>
        /// <param name="i_GameDetailsView">Contains details about the game to be created.</param>
        /// <returns>A string representing the created game room's id.</returns>
        public static async Task<String> CreateRoom(GameDetailsView i_GameDetailsView)
        {
            using (HttpClient client = new HttpClient())
            {
                String roomId = await client.PostMethodAsync("rooms/create", i_GameDetailsView);

                return roomId;
            }
        }

        /// <summary>
        /// Gets a game room by the given id string.
        /// </summary>
        public static async Task<GameRoomView> GetRoom(string i_RoomId)
        {
            using (HttpClient client = new HttpClient())
            {
                JObject room = await client.GetMethodAsync(String.Format("rooms/{0}", i_RoomId));

                return (room != null) ? room.ToObject<GameRoomView>() : null;
            }
        }

        /// <summary>
        /// Updates the view to current server values.
        /// </summary>
        public async Task Update()
        {
            GameRoomView view = await GetRoom(RoomId);

            this.DeadUsers = view.DeadUsers;
            this.LivingUsers = view.LivingUsers;
            this.Finished = view.Finished;
            this.GameTime = view.GameTime;
            this.Started = view.Started;
        }
    }
}
