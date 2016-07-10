using PhoneTag.XamarinForms.Controls.GameDetailsTile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameLobbyPage : ContentPage
    {
        private void initializeComponent(String i_GameRoomId)
        {
            NavigationPage.SetHasBackButton(this, true);

            Title = "Game Lobby";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    new GameDetailsTile(i_GameRoomId)
                }
            };
        }
    }
}
