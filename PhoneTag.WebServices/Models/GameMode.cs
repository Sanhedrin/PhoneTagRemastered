using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game mode model.
    /// </summary>
    public class GameMode : IViewable
    {
        public ObjectId _id { get; set; }
        public String Name { get; set; }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public dynamic GenerateView()
        {
            GameModeView gameModeView = new GameModeView();

            gameModeView.Name = this.Name;

            return gameModeView;
        }
    }
}