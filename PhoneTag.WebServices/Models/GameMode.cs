using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Models.GameModes;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace PhoneTag.SharedCodebase.Models
{
    /// <summary>
    /// The game mode model.
    /// </summary>
    [BsonKnownTypes(typeof(VIPGameMode), typeof(TDMGameMode))]
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