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

        public static async Task<bool> CreateRoom(GameDetailsView i_GameDetailsView)
        {
            using (HttpClient client = new HttpClient())
            {
                bool result = await client.PostMethodAsync("rooms/create", i_GameDetailsView);

                return result;
            }
        }

        public static async Task<GameRoomView> GetRoom(string i_RoomId)
        {
            using (HttpClient client = new HttpClient())
            {
                JObject room = await client.GetMethodAsync(String.Format("rooms/{0}", i_RoomId));

                return (room != null) ? room.ToObject<GameRoomView>() : null;
            }
        }

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
