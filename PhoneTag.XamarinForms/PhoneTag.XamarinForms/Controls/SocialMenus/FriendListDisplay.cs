using CrossPlatformLibrary.Extensions;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public class FriendListDisplay : PlayerListDisplay
    {
        //Initializes the player list with the the player's friends.
        public FriendListDisplay()
        {
            Refresh();
        }

        /// <summary>
        /// Refreshes the display to show more recent user status.
        /// </summary>
        public override void Refresh()
        {
            IEnumerable<UserView> playerIds = getPlayerFriends();

            updatePlayerList(playerIds, PlayerDetailsTileType.FriendList);
        }

        //Returns a collection of friend ids for the current player.
        private static IEnumerable<UserView> getPlayerFriends()
        {
            return UserView.Current?.Friends;
        }
    }
}
