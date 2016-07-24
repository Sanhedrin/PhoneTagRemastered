using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneTag.SharedCodebase.POCOs;
using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    /// <summary>
    /// A view representing a user, allows interaction with the server on per user basis.
    /// </summary>
    public class UserView : IUpdateable
    {
        private const int k_HalfAMinuteInMS = 1000 * 30;

        public static UserView Current { get; private set; }

        private CancellationTokenSource m_UserActivityCancellationToken;
        
        public String FBID { get; set; }
        public String Username { get; set; }
        public String ProfilePicUrl { get; set; }
        public List<UserView> Friends { get; set; }
        public bool IsReady { get; set; }
        public bool IsActive { get; set; }
        public int Ammo { get; set; }
        public String PlayingIn { get; set; }
        public GeoPoint CurrentLocation { get; set; }

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
        /// Sets the user's ready status
        /// </summary>
        /// <returns>The user's new ready status.</returns>
        public async Task<bool> PlayerSetReady(bool i_ReadyStatus)
        {
            using (HttpClient client = new HttpClient())
            {
                bool playerReady = await client.PostMethodAsync(String.Format("users/ready/{0}", FBID), i_ReadyStatus);

                return playerReady;
            }
        }

        /// <summary>
        /// Forces the current user to quit.
        /// Only works if already logged in.
        /// </summary>
        public void Quit()
        {
            if(m_UserActivityCancellationToken != null)
            {
                m_UserActivityCancellationToken.Cancel();
            }
            else
            {
                throw new InvalidOperationException("Can't quit on a user that's not connected.");
            }
        }

        /// <summary>
        /// Sets the user as active for other users to see them as online.
        /// This method continuously pings the server to make sure that we don't timeout.
        /// This also tries to keep the view up to date.
        /// DO NOT call this method more than once in the app's lifetime for a single user.
        /// </summary>
        public async Task Login()
        {
            if (m_UserActivityCancellationToken == null)
            {
                m_UserActivityCancellationToken = new CancellationTokenSource();
                
                //Unless we get cancelled, we'll keep pinging the server forever on occasion.
                for (;;)
                {
                    if (m_UserActivityCancellationToken.IsCancellationRequested)
                    {
                        m_UserActivityCancellationToken = null;
                        break;
                    }
                    
                    using (HttpClient client = new HttpClient())
                    {
                        await client.PostMethodAsync(String.Format("users/{0}/ping", FBID));
                    }

                    await UserView.Current.Update();

                    await Task.Delay(k_HalfAMinuteInMS);
                }
            }
        }

        /// <summary>
        /// Removes the player from the current game they're on.
        /// </summary>
        public async Task LeaveGame()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync($"users/{FBID}/leave");
            }
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
                return await client.GetMethodAsync<UserView>(String.Format("users/{0}", i_FBID));
            }
        }

        /// <summary>
        /// Called when a player should die.
        /// </summary>
        /// <returns></returns>
        public async Task Die()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync(String.Format("users/{0}/die", FBID));
            }
        }

        /// <summary>
        /// Tries to kill the given player, sending a confirmation prompt to them.
        /// </summary>
        public async Task TryKill(string i_FBID, byte[] i_KillCam)
        {
            String imageId = String.Empty;

            //First, we upload the image we just took to our image hosting service of choice
            using (HttpClient uploadClient = new HttpClient())
            {
                imageId = await uploadClient.PostImgurImageAsync(i_KillCam);
            }

            //Then we upload the kill request with the image id we just obtained as the killcam image.
            if (!String.IsNullOrEmpty(imageId))
            {
                using (HttpClient client = new HttpClient())
                {
                    await client.PostMethodAsync($"users/{UserView.Current?.FBID}/kill/{i_FBID}/{imageId}");
                }
            }
        }

        /// <summary>
        /// Updates the player's current location.
        /// </summary>
        public async Task UpdatePosition(GeoPoint i_Position)
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync<GeoPoint>($"users/{FBID}/position", i_Position);
            }
        }

        /// <summary>
        /// Updates the view to current server values.
        /// </summary>
        public async Task Update()
        {
            UserView view = await GetUser(FBID);

            this.PlayingIn = view.PlayingIn;
            this.IsReady = view.IsReady;
            this.IsActive = view.IsActive;
            this.Friends = view.Friends;
            this.Ammo = view.Ammo;
        }

    }
}
