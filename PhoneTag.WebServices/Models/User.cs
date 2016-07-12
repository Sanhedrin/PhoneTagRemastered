using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;
using System.Threading.Tasks;
using PhoneTag.WebServices.Controllers;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The user model.
    /// </summary>
    public class User : IViewable
    {
        public ObjectId _id { get; set; }
        public String FBID { get; set; }
        public String Username { get; set; }
        public String ProfilePicUrl { get; set; }
        public List<String> Friends { get; set; }
        public bool IsReady { get; set; }
        public bool IsActive { get; set; }
        public int Ammo { get; set; }
        public String PlayingIn { get; set; }

        /// <summary>
        /// Generates a view for this model,
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            UserView userView = new UserView();

            userView.FBID = FBID;
            userView.Username = Username;
            userView.ProfilePicUrl = ProfilePicUrl;
            userView.IsReady = IsReady;
            userView.IsActive = IsActive;
            userView.Ammo = Ammo;

            if (PlayingIn != null)
            {
                userView.PlayingIn = PlayingIn;
            }

            //We can't start generating views for each of my friends because it'll cause a cyclic
            //infinite loop.
            //We might not care about the entirety of the list though, and only get basic details.
            //If more information is required, the friend's username can be used to poll it.
            userView.Friends = new List<UserView>();
            foreach (String friendId in Friends)
            {
                UserView friendView = new UserView();
                User friend = await UsersController.GetUserModel(friendId);

                friendView.Username = friend.Username;
                friendView.Ammo = friend.Ammo;
                friendView.IsReady = friend.IsReady;
                friendView.Friends = null;
                friendView.IsActive = friend.IsActive;

                userView.Friends.Add(friendView);
            }

            return userView;
        }
    }
}