using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneTag.WebServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Views
{
    /// <summary>
    /// A view representing a game room, allows interaction with the server on per room basis.
    /// </summary>
    public class GameRoomView : IUpdateable
    {
        public String RoomId { get; set; }
        public GameDetailsView GameDetails { get; set; }
        public GeoPoint RoomLocation { get; set; }
        public bool Started { get; set; }

        public bool Finished { get; set; }

        public int GameTime { get; set; }
        
        public List<UserView> LivingUsers { get; private set; }
        public List<UserView> DeadUsers { get; private set; }

        public GameRoomView()
        {
            LivingUsers = new List<UserView>();
            DeadUsers = new List<UserView>();
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
                return await client.GetMethodAsync<GameRoomView>(String.Format("rooms/{0}", i_RoomId));
            }
        }

        /// <summary>
        /// Searches all the existing pending rooms in nearby proximity to the user.
        /// </summary>
        /// <param name="i_Location">Location to use as the search base.</param>
        /// <param name="i_SearchRadius">Maximum distance of the play area from the user in km.</param>
        /// <returns>A list of matching room ids</returns>
        public static async Task<List<String>> GetAllRoomsInRange(GeoPoint i_Location, float i_SearchRadius)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetMethodAsync<List<String>>(String.Format("rooms/find/{0}/{1}/{2}", i_Location.Latitude, i_Location.Longitude, i_SearchRadius));
            }
        }

        /// <summary>
        /// Adds the user with the given id to the room's player list.
        /// </summary>
        public async Task<bool> JoinRoom(string i_FBID)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.PostMethodAsync(String.Format("rooms/{0}/join/{1}", RoomId, i_FBID));
            }
        }

        /// <summary>
        /// Takes the given player out of the room.
        /// </summary>
        public async Task LeaveRoom(string i_FBID)
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync(String.Format("rooms/{0}/leave/{1}", RoomId, i_FBID));
            }
        }

        /// <summary>
        /// Gets the list of friends a specific user has in this room.
        /// </summary>
        /// <param name="i_User">User whose friends we want to poll.</param>
        public List<UserView> FriendsInRoomFor(UserView i_User)
        {
            return LivingUsers.Where((userView) => i_User.Friends.Any((friendView) => userView.Username.Equals(friendView.Username))).ToList();
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
