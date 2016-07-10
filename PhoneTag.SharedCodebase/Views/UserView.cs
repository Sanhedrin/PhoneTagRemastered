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
    /// <summary>
    /// A view representing a user, allows interaction with the server on per user basis.
    /// </summary>
    public class UserView : IUpdateable
    {
        public static UserView Current { get; private set; }
        
        public String FBID { get; set; }
        public String Username { get; set; }
        public String ProfilePicUrl { get; set; }
        public List<UserView> Friends { get; set; }
        public bool IsReady { get; set; }
        public int Ammo { get; set; }
        public bool IsActive { get; set; }
        public GameRoomView PlayingIn { get; set; }

        public UserView()
        {

        }

        /// <summary>
        /// Sets the logged in user to allow for interaction with it at any point.
        /// </summary>
        public static void SetLoggedInUser(UserView i_LoggedInUser)
        {
            Current = i_LoggedInUser;
        }

        /// <summary>
        /// Requests that a new user be created with the given name, whereas a username is unique
        /// for all users.
        /// </summary>
        public static async Task<bool> CreateUser(UserSocialView i_UserSocialView)
        {
            using (HttpClient client = new HttpClient())
            {
                bool result = await client.PostMethodAsync("users/create", i_UserSocialView);

                return result;
            }
        }

        /// <summary>
        /// Tries to get an existing user with the ID specified in the given UserSocialView.
        /// If no such user is found, a new user is created and returned.
        /// </summary>
        public static async Task<UserView> TryGetUser(UserSocialView i_UserSocialView)
        {
            UserView userView = await GetUser(i_UserSocialView.Id);

            if(userView == null)
            {
                if(await CreateUser(i_UserSocialView))
                {
                    userView = await GetUser(i_UserSocialView.Id);
                }
            }

            return userView;
        }

        /// <summary>
        /// Gets a user by the given name.
        /// </summary>
        public static async Task<UserView> GetUser(string i_FBID)
        {
            using (HttpClient client = new HttpClient())
            {
                JObject user = await client.GetMethodAsync(String.Format("users/{0}", i_FBID));

                return (user != null) ? user.ToObject<UserView>() : null;
            }
        }

        /// <summary>
        /// Updates the view to current server values.
        /// </summary>
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
