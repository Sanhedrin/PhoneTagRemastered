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

        private GameMap m_GameMap = new GameMap(new Position(32.0486850, 34.7600850), k_DefaultGameRadius, k_DefaultGameZoom);
        private CameraPreview m_Camera;

        public GamePage()
        {
            initializeComponent();

            setupPage();
        }

        private void setupPage()
        {
            m_Camera.PictureReady += GamePage_PictureReady;
        }

        private async void GamePage_PictureReady(object sender, PictureReadyEventArgs e)
        {
            await Navigation.PushAsync(new ShotDisplayPage(e.PictureBuffer));
        }

        private void GamePage_ShootButtonClicked()
        {
            m_Camera.TakePicture();
        }
    }
}