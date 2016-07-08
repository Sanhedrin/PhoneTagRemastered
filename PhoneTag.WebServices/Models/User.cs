﻿using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using MongoDB.Bson;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The user model.
    /// </summary>
    public class User : IViewable
    {
        public ObjectId _id { get; set; }
        public String Username { get; set; }
        public List<User> Friends { get; set; }
        public bool IsReady { get; set; }
        public int Ammo { get; set; }

        /// <summary>
        /// Generates a view for this model,
        /// </summary>
        public dynamic GenerateView()
        {
            UserView userView = new UserView();

            userView.Username = Username;
            userView.IsReady = IsReady;
            userView.Ammo = Ammo;

            //We can't start generating views for each of my friends because it'll cause a cyclic
            //infinite loop.
            //We might not care about the entirety of the list though, and only get basic details.
            //If more information is required, the friend's username can be used to poll it.
            userView.Friends = new List<UserView>();
            foreach (User friend in Friends)
            {
                UserView friendView = new UserView();
                friendView.Username = friend.Username;
                friendView.Ammo = friend.Ammo;
                friendView.IsReady = friend.IsReady;
                friendView.Friends = null;

                userView.Friends.Add(friendView);
            }

            return userView;
        }
    }
}