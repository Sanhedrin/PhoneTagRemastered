using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
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
        private StackLayout m_GameRoomTileDisplay = new StackLayout();

        public GameSearchPage()
        {
            initializeComponent();
        }

        protected override void OnAppearing()
        {
            m_GameRoomTileDisplay.Children.Clear();
            populateRoomList();
        }

        //Looks for all nearby rooms and adds them to the search page results.
        private async Task populateRoomList()
        {
            List<String> roomIds = await GameRoomView.GetAllRoomsInRange(new GeoPoint(0, 0), 10);

            foreach(String roomId in roomIds)
            {
                m_GameRoomTileDisplay.Children.Add(new GameDetailsTile(roomId));
            }
            
            initializeComponent();
        }
    }
}
