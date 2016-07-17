using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.WebServices.Views;
using PhoneTag.WebServices.Utils;
using PhoneTag.WebServices.Models.GameModes;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game mode model.
    /// </summary>
    public abstract class GameMode : IViewable
    {
        public ObjectId _id { get; set; }
        public String Name { get; set; }

        public abstract int TotalNumberOfPlayers { get; }

        public GameMode()
        {
            Name = GameModeFactory.GetNameOfModeViewType(GameModeModelViewRelation.GetViewTypeForModel(this.GetType()));
        }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public abstract Task<dynamic> GenerateView();
    }
}