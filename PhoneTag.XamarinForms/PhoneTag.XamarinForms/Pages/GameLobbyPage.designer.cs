using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
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

            Title = "Game Lobby";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    new GameDetailsTile(i_GameRoomId),
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
                    new Button
                    {
                        Text = "Ready",
                        BackgroundColor = Color.Red,
                        Command = new Command(() => { ReadyButton_Clicked(); })
                    }
                }
            };
        }
    }
}
