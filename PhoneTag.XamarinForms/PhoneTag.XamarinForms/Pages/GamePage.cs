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
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
using PositionEventArgs = Plugin.Geolocator.Abstractions.PositionEventArgs;
using PhoneTag.SharedCodebase.Utils;

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

            CrossGeolocator.Current.PositionChanged += GPS_PositionChanged;
        }

        //When our position changes, we should inform the server about it.
        private void GPS_PositionChanged(object sender, PositionEventArgs e)
        {
            GeoPoint position = new GeoPoint(e.Position.Latitude, e.Position.Longitude);
            UserView.Current.UpdatePosition(position);
        }

        private void GamePage_PictureReady(object sender, PictureReadyEventArgs e)
        {
            pictureReady(e.PictureBuffer);
        }

        private async Task pictureReady(byte[] i_PictureData)
        {
            ShotDisplayPage shotDisplayPage = new ShotDisplayPage(i_PictureData);
            shotDisplayPage.ShotCancelled += (o, e) =>
            {
                buttonShoot.IsEnabled = true;
                buttonShoot.Text = "Shoot!";
            };

            await Navigation.PushAsync(shotDisplayPage);
        }

        private void ShootButton_Clicked(object sender, EventArgs e)
        {
            if (buttonShoot.Text.Equals("Shoot!"))
            {
                buttonShoot.IsEnabled = false;
                buttonShoot.Text = "Processing shot...";

                m_Camera.TakePicture();
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                UserView.Current.LeaveGame();
            }
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            if(i_EventDetails is KillRequestEvent)
            {
                handleKillRequestEvent(i_EventDetails as KillRequestEvent);
            }
            else if(i_EventDetails is PlayerKilledEvent)
            {
                handlePlayerKilledEvent(i_EventDetails as PlayerKilledEvent);
            }
        }

        //Triggers when a player dies.
        private void handlePlayerKilledEvent(PlayerKilledEvent i_PlayerKilledEvent)
        {
            displayNotification(i_PlayerKilledEvent);
        }

        //Triggers when another player issues a kill command on you.
        private void handleKillRequestEvent(KillRequestEvent i_KillRequestEvent)
        {
            KillConfirmationDialog killConfirmationDialog = new KillConfirmationDialog(i_KillRequestEvent);

            killConfirmationDialog.KillConfirmed += KillConfirmationDialog_KillConfirmed;
            killConfirmationDialog.KillDenied += KillConfirmationDialog_KillDenied;

            showDialog(killConfirmationDialog);
        }
        
        private void KillConfirmationDialog_KillDenied(object sender, EventArgs e)
        {
            hideDialog();
        }

        private void KillConfirmationDialog_KillConfirmed(object sender, EventArgs e)
        {
            playerKilled();
        }

        //Kill the current player and remove them from the game.
        private async Task playerKilled()
        {
            await UserView.Current.Die();

            await hideDialog();

            await transitionToSpectatorMode();
        }
    }
}