using PhoneTag.XamarinForms.Controls;
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
    public partial class GamePage : ContentPage
    {
        private GameMap m_GameMap = new GameMap(new Position(32.0486850, 34.7600850), 0.5, 1);

        public GamePage()
        {
            initializeComponent();

            setupPage();
        }

        private void setupPage()
        {
            m_Camera.PictureReady += GamePage_PictureReady;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GamePage_PictureReady(object sender, PictureReadyEventArgs e)
        {
            await Navigation.PushAsync(new ShotDisplayPage(e.PictureBuffer));
        }
    }
}