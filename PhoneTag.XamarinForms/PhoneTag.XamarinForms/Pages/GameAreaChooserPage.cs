using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.XamarinForms.Controls.MapControl;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using GpsPosition = Plugin.Geolocator.Abstractions.Position;
using MapPosition = Xamarin.Forms.Maps.Position;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// This page displays an interactive map that allows you to choose the game area while setting
    /// up a game.
    /// The chosen area is accessible via the static properties after returning from this page.
    /// </summary>
    public partial class GameAreaChooserPage : TrailableContentPage
    {
        /// <summary>
        /// Holds the game location as chosen by the interactive map.
        /// </summary>
        public MapPosition ChosenPosition { get; private set; }
        /// <summary>
        /// Holds the game radius as chosen by the interactive map.
        /// </summary>
        public double ChosenRadius { get; private set; }

        private const double k_DefaultGameRadius = 0.5;
        private const double k_DefaultGameZoom = 1;
        private const bool k_IsSetUpView = true;

        private GameMapSetup m_GameMap;
        
        public GameAreaChooserPage() : base()
        {
            setupChooserMap();
        }

        //Initializes the map to the last chosen location or to your current location.
        private async Task setupChooserMap()
        {
            await startGeoLocationListening();

            GpsPosition userLocation = await CrossGeolocator.Current.GetPositionAsync(timeoutMilliseconds: 3);

            await CrossGeolocator.Current.StopListeningAsync();

            MapPosition startLocation = new MapPosition(userLocation.Latitude, userLocation.Longitude);
            m_GameMap = new GameMapSetup(startLocation, k_DefaultGameRadius, k_DefaultGameZoom);
           
            //Store the values in the static properties for access once we're done.
            ChosenPosition = m_GameMap.StartLocation = startLocation;
            ChosenRadius = m_GameMap.GameRadius = k_DefaultGameRadius;

            initializeComponent();
        }

        //When the area is chosen return to the last page.
        private async Task DoneButton_Clicked()
        {
            ChosenPosition = m_GameMap.StartLocation;
            ChosenRadius = m_GameMap.GameRadius;
            await Navigation.PopAsync();
        }

        //Starts listening to the geolocator, while looking for errors.
        private async Task startGeoLocationListening()
        {
            if (!CrossGeolocator.Current.IsListening)
            {
                bool isReady = false;

                if (CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled)
                {
                    isReady = await CrossGeolocator.Current.StartListeningAsync(1, 1);
                }

                if (!isReady)
                {
                    Application.Current.MainPage = new ErrorPage("GPS signal not found, please enable GPS");
                }
            }
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            throw new NotImplementedException();
        }
    }
}
