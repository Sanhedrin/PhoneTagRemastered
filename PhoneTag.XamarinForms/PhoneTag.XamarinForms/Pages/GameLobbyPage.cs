using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameLobbyPage : TrailableContentPage
    {
        private bool ReadyRequestComplete { get; set; }

        private GameRoomView m_GameRoom;

        private Button buttonReady;

        public GameLobbyPage(String i_GameRoomId) : base()
        {
            initializeLoadingComponent();
            joinRoom(i_GameRoomId);
        }
        
        //Upon opening a room's lobby page, we essentialy enter it.
        private async Task joinRoom(String i_GameRoomId)
        {
            m_GameRoom = await GameRoomView.GetRoom(i_GameRoomId);
            await m_GameRoom.JoinRoom(UserView.Current.FBID);
            await UserView.Current.Update();

            initializeComponent(i_GameRoomId);
        }


        private void ButtonReady_Clicked(object sender, EventArgs e)
        {
            buttonReady.IsEnabled = false;
            triggerReadyStatus();
        }

        //Changes the ready status of the player in the room.
        private async Task triggerReadyStatus()
        {
            await UserView.Current.Update();

            UserView.Current.IsReady = await UserView.Current.PlayerSetReady(!UserView.Current.IsReady);

            buttonReady.IsEnabled = true;
            buttonReady.Text = UserView.Current.IsReady ? "Unready" : "Ready";
        }

        public override void ParseEvent(Event i_Event)
        {
            if (i_Event is GameStartEvent)
            {
                startGame(i_Event as GameStartEvent);
            }
        }

        private async Task startGame(GameStartEvent i_GameStartEvent)
        {
            GameRoomView roomView = await GameRoomView.GetRoom(i_GameStartEvent.GameId);

            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new GamePage(roomView));
        }
    }
}
