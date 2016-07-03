using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    public class UserView : IUpdateable
    {
        public String Username { get; set; }
        public List<UserView> Friends { get; set; }
        public bool IsReady { get; set; }
        public int Ammo { get; set; }
        public bool IsActive { get; set; }
        public GameRoomView PlayingIn { get; set; }

        public UserView()
        {

        }

        public static async Task<bool> CreateUser(string i_Username)
        {
            using (HttpClient client = new HttpClient())
            {
                bool result = await client.PostMethodAsync("users/create", i_Username);

                return result;
            }
        }

        public static async Task<UserView> GetUser(string i_Username)
        {
            using (HttpClient client = new HttpClient())
            {
                JObject user = await client.GetMethodAsync(String.Format("users/{0}", i_Username));

                return (user != null) ? user.ToObject<UserView>() : null;
            }
        }

        public async Task Update()
        {
            UserView view = await GetUser(Username);

            this.PlayingIn = view.PlayingIn;
            this.IsReady = view.IsReady;
            this.IsActive = view.IsActive;
            this.Friends = view.Friends;
            this.Ammo = view.Ammo;
        }
    }
}
