using PhoneTag.WebServices.Views;
using PhoneTag.XamarinForms.Controls.MapControl;
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
        private bool ReadyRequestComplete { get; set; }

        private GameRoomView m_GameRoom;

        private Button buttonReady;

        public GameLobbyPage(String i_GameRoomId)
        {
            joinRoom(i_GameRoomId);
        }

        protected override void OnDisappearing()
        {
            leaveRoom();

            base.OnDisappearing();
        }

        private async void leaveRoom()
        {
            if (m_GameRoom == null) await Task.Delay(5000);

            m_GameRoom?.LeaveRoom(UserView.Current.FBID);
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
            
            //Application.Current.MainPage = new NavigationPage(new GamePage(m_GameRoom));
        }
    }
}
