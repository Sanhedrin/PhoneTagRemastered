using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public abstract partial class PlayerListDisplay : ScrollView
    {
        protected List<UserView> m_Players = new List<UserView>();

        protected PlayerListDisplay()
        {
            initializeLoadingComponent();
            Refresh();
        }

        protected void updatePlayerList(IEnumerable<UserView> i_Players, PlayerDetailsTileType i_DetailsType)
        {
            if (i_Players != null && i_Players.Count() > 0)
            {
                m_Players = i_Players.ToList();
            }

            initializeComponent(i_DetailsType);
        }

        //Creates a list containing player detail tiles for display.
        protected async Task<StackLayout> generatePlayerListPresenter(PlayerDetailsTileType i_DetailType)
        {
            StackLayout playerDetailsList = new StackLayout();

            foreach(UserView user in m_Players)
            {
                await user.Update();
                playerDetailsList.Children.Add(PlayerDetailsTileFactory.GetPlayerDetailsTileFor(i_DetailType, user));
            }

            return playerDetailsList;
        }

        /// <summary>
        /// Refreshes the display to show more recent user status.
        /// </summary>
        public abstract void Refresh();
    }
}
