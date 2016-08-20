using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    /// <summary>
    /// A UI tile that displays relevant information about the given player.
    /// </summary>
    public abstract partial class PlayerDetailsTile : RelativeLayout
    {
        public UserView UserView { get; set; }

        public PlayerDetailsTile(UserView i_UserView)
        {
            UserView = i_UserView;

            initializeComponent();
        }

        protected abstract void setupTile();

        public abstract void Refresh(PlayerDetailsTile i_PlayerDetails);
    }
}
