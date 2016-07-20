using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
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
        private async Task showFriendMenu()
        {
            await this.ScaleTo(0.8, 75, Easing.CubicOut);
            await this.ScaleTo(1, 75, Easing.CubicIn);

            if (this.Parent != null && this.Parent.GetType().Equals(typeof(AbsoluteLayout)))
            {
                FriendListDisplay friendList = new FriendListDisplay();

                AbsoluteLayout.SetLayoutBounds(friendList, new Rectangle(0.9, 0.1, 0.8, 0.8));
                AbsoluteLayout.SetLayoutFlags(friendList, AbsoluteLayoutFlags.All);

                ((AbsoluteLayout)this.Parent).Children.Add(friendList);
            }
            else
            {
                throw new InvalidOperationException("Friend list button must be childed by an AbsoluteLayout");
            }
        }
    }
}
