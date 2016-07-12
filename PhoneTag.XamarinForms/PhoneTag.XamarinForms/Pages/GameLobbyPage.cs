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
    public partial class GameLobbyPage : ContentPage
    {
        private GameRoomView m_GameRoom;

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
            await m_GameRoom?.LeaveRoom(UserView.Current.FBID);
        }

        //Upon opening a room's lobby page, we essentialy enter it.
        private async Task joinRoom(String i_GameRoomId)
        {
            m_GameRoom = await GameRoomView.GetRoom(i_GameRoomId);
            await m_GameRoom.JoinRoom(UserView.Current.FBID);
            await UserView.Current.Update();

            initializeComponent(i_GameRoomId);
        }

        private void ReadyButton_Clicked()
        {
            Application.Current.MainPage = new NavigationPage(new GamePage(m_GameRoom));
        }
    }
}
