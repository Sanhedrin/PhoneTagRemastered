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
