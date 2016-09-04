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
using PhoneTag.XamarinForms.Controls.SocialMenu;
using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.Controls.MenuButtons;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameLobbyPage : ChatEmbeddedContentPage
    {
        private LobbyPlayerListDisplay m_LobbyPlayerList;

        private void initializeLoadingComponent()
        {
            BackgroundColor = Color.Black;

            Title = "Loading";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new AbsoluteLayout();

            StackLayout lobbyLayout = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new AnimatedImage()
                    {
                        ImageName = "loading_logo",
                        Animate = true,
                        AnimationFrames = 30
                    },
                    new Label
                    {
                        Text = "Loading...",
                        TextColor = Color.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
        }

        private async Task initializeComponent(String i_GameRoomId)
        {
            NavigationPage.SetHasBackButton(this, true);

            if (i_GameRoomId != null)
            {
                m_GameRoom = await GameRoomView.GetRoom(i_GameRoomId);

                if (m_GameRoom != null)
                {
                    Title = "Game Lobby";
                    Padding = new Thickness(0, 20, 0, 0);
                    
                    StackLayout lobbyLayout = await generateLobbyLayout(i_GameRoomId);
                    AbsoluteLayout.SetLayoutBounds(lobbyLayout, new Rectangle(0, 0, lobbyLayout.Width, lobbyLayout.Height));
                    
                    Content = new AbsoluteLayout();
                    (Content as AbsoluteLayout).Children.Add(lobbyLayout);

                    initializeChat();
                }
            }
        }

        private async Task<StackLayout> generateLobbyLayout(String i_GameRoomId)
        {
            GameDetailsTile roomTile = await generateRoomTile(i_GameRoomId);
            m_ButtonReady = generateReadyButton();
            Button viewMapButton = generateViewMapButton();
            m_LobbyPlayerList = (LobbyPlayerListDisplay)(m_LobbyPlayerList?.Refresh()) ?? new LobbyPlayerListDisplay();

            m_ButtonReady.IsEnabled = m_GameRoom.LivingUsers.Count > 1;

            StackLayout lobbyLayout = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    roomTile,
                    m_LobbyPlayerList,
                    viewMapButton,
                    m_ButtonReady
                }
            };

            return lobbyLayout;
        }

        private Button generateViewMapButton()
        {
            Position startLocation = new Position(m_GameRoom.GameDetails.StartLocation.Latitude, m_GameRoom.GameDetails.StartLocation.Longitude);

            Button button = new Button
            {
                Text = "View Map",
                TextColor = Color.Black,
                BackgroundColor = Color.Yellow,
                Command = new Command(() =>
                {
                    Navigation.PushAsync(new GameAreaDisplayPage(startLocation, m_GameRoom.GameDetails.GameRadius));
                })
            };

            return button;
        }

        private Button generateReadyButton()
        {
            m_ButtonReady = new Button()
            {
                Text = m_ButtonReady?.Text ?? "Ready",
                TextColor = Color.Black,
                BackgroundColor = Color.Red
            };

            m_ButtonReady.BindingContext = this;
            //buttonReady.SetBinding(Button.IsEnabledProperty, "ReadyRequestComplete");
            m_ButtonReady.Clicked += ButtonReady_Clicked;

            return m_ButtonReady;
        }

        private async Task<GameDetailsTile> generateRoomTile(String i_GameRoomId)
        {
            GameDetailsTile roomTile = new GameDetailsTile();

            await roomTile.SetupTile(i_GameRoomId);

            return roomTile;
        }
    }
}
