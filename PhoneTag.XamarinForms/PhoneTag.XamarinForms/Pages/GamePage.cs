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
using PhoneTag.WebServices.Views;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game page, 
    /// </summary>
    public partial class GamePage : ContentPage
    {
        private const double k_DefaultGameRadius = 0.5;
        private const double k_DefaultGameZoom = 1;
        private const bool k_IsSetUpView = false;

        private GameMapInteractive m_GameMap;
        private CameraPreview m_Camera;
        private GameRoomView m_GameRoomView;

        public GamePage(GameRoomView i_GameRoomView)
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
    }
}