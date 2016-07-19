using Plugin.XamJam.Screen;
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
        private void initializeComponent()
        {
            Source = "friend_menu_icon.png";

            VerticalOptions = HorizontalOptions = new LayoutOptions
            {
                Alignment = LayoutAlignment.Fill
            };

            double cubicSize = CrossScreen.Current.Size.Width / 8;
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.95f, 0.95f, cubicSize, cubicSize));
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}
