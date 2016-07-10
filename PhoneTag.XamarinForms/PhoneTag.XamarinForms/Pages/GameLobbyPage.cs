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
        public GameLobbyPage(String i_GameRoomId)
        {
            initializeComponent(i_GameRoomId);
        }
    }
}
