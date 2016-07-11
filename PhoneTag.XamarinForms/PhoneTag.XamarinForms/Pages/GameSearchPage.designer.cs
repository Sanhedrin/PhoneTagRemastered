using PhoneTag.XamarinForms.Controls.GameDetailsTile;
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
        private void initializeComponent()
        {
            NavigationPage.SetHasBackButton(this, true);

            m_GameRoomTileDisplay.VerticalOptions = new LayoutOptions{ Alignment = LayoutAlignment.Fill };
            
            Title = "Find a game near you";
            Padding = new Thickness(0, 20, 0, 0);
            Content = m_GameRoomTileDisplay;
        }
    }
}
