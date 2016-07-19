using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.FriendMenu
{
    public partial class FriendListButton : Image
    {
        public FriendListButton()
        {
            initializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            showFriendMenu();
        }

        //Shows the friend menu.
        private void showFriendMenu()
        {
        }
    }
}
