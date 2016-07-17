using FreshEssentials;
using Nito.AsyncEx;
using PhoneTag.WebServices.Utils;
using PhoneTag.WebServices.Views;
using PhoneTag.XamarinForms.Controls.GameDetailsTile;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameSearchPage : ContentPage
    {
        public int SearchRadius { get; set; }

        private BindablePicker pickerSearchRadius;

        private StackLayout m_GameRoomTileDisplay = new StackLayout();

        private static readonly AsyncLock sr_PopulateRoomMutex = new AsyncLock();

        public GameSearchPage()
        {
            initRadiusPicker();
            initializeComponent();
        }

        protected override void OnAppearing()
        {
            CrossGeolocator.Current.PositionError += Current_PositionError;

            populateRoomList();
        }

        private void Current_PositionError(object sender, PositionErrorEventArgs e)
        {
            Application.Current.MainPage = new ErrorPage("GPS signal not found, please enable GPS");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        //Looks for all nearby rooms and adds them to the search page results.
        private async Task populateRoomList()
        {
            using(await sr_PopulateRoomMutex.LockAsync())
            {
                m_GameRoomTileDisplay.Children.Clear();

                await startGeoLocationListening();

                Position userLocation = await CrossGeolocator.Current.GetPositionAsync(timeoutMilliseconds: 3);

                await CrossGeolocator.Current.StopListeningAsync();

                List<String> roomIds = await GameRoomView.GetAllRoomsInRange(new GeoPoint(userLocation.Latitude, userLocation.Longitude), SearchRadius);
                
                await UserView.Current.Update();

                foreach (String roomId in roomIds)
                {
                    GameDetailsTile tile = new GameDetailsTile();
                    await tile.SetupTile(roomId);
                    m_GameRoomTileDisplay.Children.Add(tile);
                }

                initializeComponent();
            }
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
    }
}
