using PhoneTag.SharedCodebase.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    public enum PlayerDetailsTileType
    {
        Lobby,
        FriendList
    }

    public static class PlayerDetailsTileFactory
    {
        /// <summary>
        /// Generates a details tile of the given user in accordance to the formatting necessary for the 
        /// given menu type.
        /// </summary>
        public static PlayerDetailsTile GetPlayerDetailsTileFor(PlayerDetailsTileType i_TileType, UserView i_UserView)
        {
            PlayerDetailsTile tile = null;

            switch (i_TileType)
            {
                case PlayerDetailsTileType.FriendList:
                    tile = new FriendDetailsTile(i_UserView);
                    break;
                case PlayerDetailsTileType.Lobby:
                    tile = new LobbyPlayerDetailsTile(i_UserView);
                    break;
            }

            return tile;
        }
    }
}
