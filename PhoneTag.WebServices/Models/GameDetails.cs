using MongoDB.Bson;
using PhoneTag.SharedCodebase.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The game details model.
    /// </summary>
    public class GameDetails : IViewable
    {
        public ObjectId _id { get; set; }

        public int GpsRefreshRate { get; set; }

        /// <summary>
        /// Generates a view of this model.
        /// </summary>
        public dynamic GenerateView()
        {
            GameDetailsView detailsView = new GameDetailsView();

            detailsView.GpsRefreshRate = GpsRefreshRate;

            return detailsView;
        }

        /// <summary>
        /// Generates a mode from the given view.
        /// </summary>
        public static GameDetails FromView(GameDetailsView i_GameDetailsView)
        {
            GameDetails details = new GameDetails();

            details.GpsRefreshRate = i_GameDetailsView.GpsRefreshRate;

            return details;
        }
    }
}
