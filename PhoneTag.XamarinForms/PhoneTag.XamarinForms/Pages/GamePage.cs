using PhoneTag.XamarinForms.Controls.MapControl;
using PhoneTag.XamarinForms.Controls.CameraControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Events.GameEvents;
using Plugin.Geolocator;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game page, 
    /// </summary>
    public partial class GamePage : TrailableContentPage
    {
        private const double k_DefaultGameRadius = 0.5;
        private const double k_DefaultGameZoom = 1;
        private const bool k_IsSetUpView = false;

        private GameMapInteractive m_GameMap;
        private CameraPreview m_Camera;
        private GameRoomView m_GameRoomView;

        public GamePage(GameRoomView i_GameRoomView) : base()
        {
            m_GameRoomView = i_GameRoomView;
            Position startLocation = new Position(m_GameRoomView.GameDetails.StartLocation.Latitude, m_GameRoomView.GameDetails.StartLocation.Longitude);
            m_GameMap = new GameMapInteractive(startLocation, m_GameRoomView.GameDetails.GameRadius, m_GameRoomView.GameDetails.GameRadius * 2);

            initializeComponent();

            setupPage();
        }

        private void setupPage()
        {
            m_Camera.PictureReady += GamePage_PictureReady;

            startGeoLocationListening();
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

        private void GamePage_PictureReady(object sender, PictureReadyEventArgs e)
        {
            pictureReady(e.PictureBuffer);
        }

        private async Task pictureReady(byte[] i_PictureData)
        {
            await Navigation.PushAsync(new ShotDisplayPage(i_PictureData));
        }

        private void ShootButton_Clicked()
        {
            m_Camera.TakePicture();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            throw new NotImplementedException();
        }
    }
}