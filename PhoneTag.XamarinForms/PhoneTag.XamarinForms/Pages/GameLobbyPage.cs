using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu;
using PhoneTag.XamarinForms.Controls.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Geolocator;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameLobbyPage : TrailableContentPage
    {
        private bool ReadyRequestComplete { get; set; }

        private GameRoomView m_GameRoom;
        
        public IEnumerable<UserView> PlayerList
        {
            get
            {
                return m_GameRoom.DeadUsers.Union(m_GameRoom.LivingUsers);
            }
        }

        private Button buttonReady;

        public GameLobbyPage(String i_GameRoomId) : base()
        {
            initializeLoadingComponent();
            joinRoom(i_GameRoomId);
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (m_GameRoom != null && !m_GameRoom.Started && UserView.Current?.FBID != null)
            {
                m_GameRoom.LeaveRoom(UserView.Current.FBID);
            }
        }

        //Upon opening a room's lobby page, we essentialy enter it.
        private async Task joinRoom(String i_GameRoomId)
        {
            if (i_GameRoomId != null)
            {
                m_GameRoom = await GameRoomView.GetRoom(i_GameRoomId);

                if (m_GameRoom != null)
                {
                    await m_GameRoom.JoinRoom(UserView.Current.FBID);
                    await UserView.Current.Update();

                    initializeComponent(i_GameRoomId);
                }
                else
                {
                    Application.Current.MainPage = new ErrorPage("Couldn't find requested game room.");
                }
            }
            else
            {
                Application.Current.MainPage = new ErrorPage("Couldn't find requested game room.");
            }
        }


        private void ButtonReady_Clicked(object sender, EventArgs e)
        {
            if (CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled)
            {
                buttonReady.IsEnabled = false;
                triggerReadyStatus();
            }
            else
            {
                DisplayAlert("Can't participate in a game!", $"No GPS signal found.{Environment.NewLine}Please try enabling your GPS and then try again.", "Ok");
            }
        }

        //Changes the ready status of the player in the room.
        private async Task triggerReadyStatus()
        {
            if (UserView.Current != null)
            {
                await UserView.Current.Update();

                UserView.Current.IsReady = await UserView.Current.PlayerSetReady(!UserView.Current.IsReady);

                buttonReady.IsEnabled = true;
                buttonReady.Text = UserView.Current.IsReady ? "Unready" : "Ready";
            }
        }

        public override void ParseEvent(Event i_Event)
        {
            if (i_Event is GameStartEvent)
            {
                startGame(i_Event as GameStartEvent);
            }
            else if (i_Event is GameLobbyUpdateEvent)
            {
                updateGameLobby(i_Event as GameLobbyUpdateEvent);
            }
        }

        private void updateGameLobby(GameLobbyUpdateEvent i_GameLobbyUpdateEvent)
        {
            if (i_GameLobbyUpdateEvent.GameId.Equals(m_GameRoom.RoomId))
            {
                initializeComponent(i_GameLobbyUpdateEvent.GameId);
            }
            else
            {
#if DEBUG
                throw new Exception("Event GameId doesn't fit current GameId.");
#endif
            }
        }

        private async Task startGame(GameStartEvent i_GameStartEvent)
        {
            m_GameRoom = await GameRoomView.GetRoom(i_GameStartEvent.GameId);

            if (m_GameRoom != null)
            {
                Application.Current.MainPage = new NavigationPage(new GamePage(m_GameRoom));
            }
            else
            {
                Application.Current.MainPage = new ErrorPage("Couldn't find specified game.");
            }
        }
    }
}
