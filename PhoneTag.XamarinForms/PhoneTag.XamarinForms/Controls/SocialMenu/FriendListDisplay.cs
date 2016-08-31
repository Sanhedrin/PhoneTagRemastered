using CrossPlatformLibrary.Extensions;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public class FriendListDisplay : PlayerListDisplay
    {
        public FriendListDisplay() : base()
        {
            BackgroundColor = Color.Gray;
            Padding = new Thickness { Left = CrossScreen.Current.Size.Width / 50, Right = CrossScreen.Current.Size.Width / 50 };
        }



        //Creates a list containing player detail tiles for display.
        protected override async Task<StackLayout> generatePlayerListPresenter(PlayerDetailsTileType i_DetailType)
        {
            StackLayout playerDetailsList = new StackLayout();

            playerDetailsList.Children.Add(new Label() { Text = "Online friends:", BackgroundColor = Color.Black, TextColor = Color.White });
            foreach (UserView user in m_Players)
            {
                await user.Update();

                if (user.IsActive)
                {
                    playerDetailsList.Children.Add(PlayerDetailsTileFactory.GetPlayerDetailsTileFor(i_DetailType, user));
                }
            }

            playerDetailsList.Children.Add(new Label() { Text = "Offline friends:", BackgroundColor = Color.Black, TextColor = Color.White });
            foreach (UserView user in m_Players)
            {
                await user.Update();

                if (!user.IsActive)
                {
                    playerDetailsList.Children.Add(PlayerDetailsTileFactory.GetPlayerDetailsTileFor(i_DetailType, user));
                }
            }

            return playerDetailsList;
        }

        /// <summary>
        /// Refreshes the display to show more recent user status.
        /// </summary>
        public override PlayerListDisplay Refresh()
        {
            IEnumerable<UserView> playerIds = getPlayerFriends();

            updatePlayerList(playerIds, PlayerDetailsTileType.FriendList);

            return this;
        }

        //Returns a collection of friend ids for the current player.
        private IEnumerable<UserView> getPlayerFriends()
        {
            return UserView.Current?.Friends;
        }
    }
}
