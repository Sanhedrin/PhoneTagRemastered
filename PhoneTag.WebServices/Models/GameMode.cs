using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.WebServices.Models.GameModes;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game mode model.
    /// </summary>
    [BsonKnownTypes(typeof(TDMGameMode), typeof(VIPGameMode))]
    public abstract class GameMode : IViewable
    {
        public ObjectId _id { get; set; }
        public String Name { get; set; }
        public List<List<String>> Teams { get; private set; }

        public abstract int TotalNumberOfPlayers { get; }

        public GameMode()
        {
            Name = GameModeFactory.GetNameOfModeViewType(GameModeModelViewRelation.GetViewTypeForModel(this.GetType()));
            Teams = new List<List<string>>();
        }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public abstract Task<dynamic> GenerateView();

        /// <summary>
        /// Arranges the teams for this game according to the mode's rules.
        /// </summary>
        public abstract void ArrangeTeams(List<string> i_LivingUsers);

        /// <summary>
        /// Gets the enemies of the given player
        /// </summary>
        public List<string> GetEnemiesFor(String i_FBID)
        {
            IEnumerable<String> enemies = new List<string>();

            foreach(List<String> team in Teams)
            {
                if (!team.Contains(i_FBID))
                {
                    enemies = enemies.Union(team);
                }
            }

            return enemies.Count() > 0 ? enemies.ToList() : new List<string>();
        }

        /// <summary>
        /// Gets the teammates of the given player
        /// </summary>
        public List<string> GetTeammatesFor(String i_FBID)
        {
            IEnumerable<String> enemies = new List<string>();

            foreach (List<String> team in Teams)
            {
                if (team.Contains(i_FBID))
                {
                    enemies = enemies.Union(team);
                }
            }
            
            return enemies.Count() > 0 ? enemies.ToList() : new List<string>();
        }
    }
}