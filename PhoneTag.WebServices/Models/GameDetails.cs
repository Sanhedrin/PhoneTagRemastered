using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Views.GameModes;
using PhoneTag.SharedCodebase.Models.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Models
{
    /// <summary>
    /// The game details model.
    /// </summary>
    public class GameDetails : IViewable
    {
        public ObjectId _id { get; set; }

        public String Name { get; set; }
        public int GpsRefreshRate { get; set; }
        public int GameDurationInMins { get; set; }
        public GeoJsonPoint<GeoJson2DCoordinates> StartLocation { get; set; }
        public double GameRadius { get; set; }

        public GameMode Mode { get; set; }

        /// <summary>
        /// Generates a view of this model.
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            GameDetailsView detailsView = new GameDetailsView();

            detailsView.Name = Name;
            detailsView.GpsRefreshRate = GpsRefreshRate;
            detailsView.GameDurationInMins = GameDurationInMins;
            detailsView.GameRadius = GameRadius;

            if (StartLocation != null)
            {
                detailsView.StartLocation = new GeoPoint(StartLocation.Coordinates.Y, StartLocation.Coordinates.X);
            }

            detailsView.Mode = await Mode.GenerateView();

            return detailsView;
        }

        /// <summary>
        /// Generates a mode from the given view.
        /// </summary>
        public static GameDetails FromView(GameDetailsView i_GameDetailsView)
        {
            GameDetails details = new GameDetails();

            details.Name = i_GameDetailsView.Name;
            details.GpsRefreshRate = i_GameDetailsView.GpsRefreshRate;
            details.GameDurationInMins = i_GameDetailsView.GameDurationInMins;
            details.StartLocation = new GeoJsonPoint<GeoJson2DCoordinates>(
                new GeoJson2DCoordinates(i_GameDetailsView.StartLocation.Longitude, i_GameDetailsView.StartLocation.Latitude));
            details.GameRadius = i_GameDetailsView.GameRadius;

            //We want to invoke the FromView method like so:
            //details.Mode = GameMode.FromView(i_GameDetailsView.Mode);
            //However, since FromView is a static method, we can't use an overriden method from a specific
            //game mode type when the type of game mode is not known at compile time, hence why we
            //use reflection to invoke the FromView method of the correct type.
            //Null in the first argument means that we're using a static method(Since there's no instance).
            object castedMode = Convert.ChangeType(i_GameDetailsView.Mode, i_GameDetailsView.Mode.GetType());
            Type modeModel= GameModeModelViewRelation.GetModelTypeForView(i_GameDetailsView.Mode.GetType());
            details.Mode = (GameMode)modeModel.GetMethod("FromView").Invoke(null, new object[] { castedMode });

            return details;
        }
    }
}
