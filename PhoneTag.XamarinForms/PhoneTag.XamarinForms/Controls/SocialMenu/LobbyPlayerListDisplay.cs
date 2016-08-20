using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using PhoneTag.XamarinForms.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public class LobbyPlayerListDisplay : PlayerListDisplay
    {
        /// <summary>
        /// Refreshes the display to show more recent user status.
        /// </summary>
        public override PlayerListDisplay Refresh()
        {
            IEnumerable<UserView> players = getLobbyPlayers();

            updatePlayerList(players, PlayerDetailsTileType.Lobby);

            return this;
        }

        //Returns a collection of friend ids for the current player.
        private static IEnumerable<UserView> getLobbyPlayers()
        {
            GameLobbyPage lobbyPage = TrailableContentPage.CurrentPage as GameLobbyPage;

            //This component should not be used outside of the lobby room.
            if (lobbyPage == null)
            {
                throw new InvalidOperationException("LobbyPlayerList component used in non-lobby room.");
            }

            return lobbyPage.PlayerList;
        }
    }
}
