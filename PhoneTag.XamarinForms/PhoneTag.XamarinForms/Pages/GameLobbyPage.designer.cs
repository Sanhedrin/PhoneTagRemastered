using PhoneTag.WebServices.Utils;
using PhoneTag.WebServices.Views;
using PhoneTag.XamarinForms.Controls.GameDetailsTile;
using PhoneTag.XamarinForms.Controls.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameLobbyPage : ContentPage
    {
        private async void initializeComponent(String i_GameRoomId)
        {
            NavigationPage.SetHasBackButton(this, true);
            GameRoomView gameRoom = await GameRoomView.GetRoom(i_GameRoomId);

            Position startLocation = new Position(gameRoom.GameDetails.StartLocation.Latitude, gameRoom.GameDetails.StartLocation.Longitude);

            GameDetailsTile roomTile = new GameDetailsTile();
            await roomTile.SetupTile(i_GameRoomId);

            buttonReady = new Button()
            {
                Text = "Ready",
                BackgroundColor = Color.Red
            };
            buttonReady.BindingContext = this;
            buttonReady.SetBinding(Button.IsEnabledProperty, "ReadyRequestPending");
            buttonReady.Clicked += ButtonReady_Clicked;

            Title = "Game Lobby";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    roomTile,
                    //TODO: Insert player list
                    //TODO: Insert chat box.
                    new Button
                    {
                        Text = "View Map",
                        BackgroundColor = Color.Yellow,
                        Command = new Command(() => {
                            Navigation.PushAsync(new GameAreaDisplayPage(startLocation, gameRoom.GameDetails.GameRadius)); 
                        })
                    },
                    buttonReady
                }
            };
        }
    }
}
