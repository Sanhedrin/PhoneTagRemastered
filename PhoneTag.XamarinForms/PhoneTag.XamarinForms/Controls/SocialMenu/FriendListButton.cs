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
        private FriendListDisplay m_FriendDisplay = null;
        private Image m_MenuBackground;

        public FriendListButton()
        {
            initializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TriggerFriendMenu();
        }

        public void TriggerFriendMenu()
        {
            if (m_FriendDisplay == null)
            {
                showFriendMenu();
            }
            else
            {
                closeFriendMenu();
            }
        }

        //Shows the friend menu.
        private async Task showFriendMenu()
        {
            await this.ScaleTo(0.8, 75, Easing.CubicOut);
            await this.ScaleTo(1, 75, Easing.CubicIn);

            if (this.Parent != null && this.Parent.GetType().Equals(typeof(AbsoluteLayout)))
            {
                m_FriendDisplay = new FriendListDisplay();
                m_MenuBackground = new Image(){ Source = ImageSource.FromFile("killcam_frame.png"), Aspect = Aspect.Fill };
                
                AbsoluteLayout.SetLayoutBounds(m_FriendDisplay, new Rectangle(0.30, 10.5, 0.65, 0.9));
                AbsoluteLayout.SetLayoutFlags(m_FriendDisplay, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(m_MenuBackground, new Rectangle(0.15, 10, 0.8, 0.9));
                AbsoluteLayout.SetLayoutFlags(m_MenuBackground, AbsoluteLayoutFlags.All);

                ((AbsoluteLayout)this.Parent).Children.Add(m_FriendDisplay);
                ((AbsoluteLayout)this.Parent).Children.Add(m_MenuBackground);

                m_FriendDisplay.TranslateTo(0, -m_FriendDisplay.Height, 250, Easing.Linear);
                await m_MenuBackground.TranslateTo(0, -m_FriendDisplay.Height, 250, Easing.Linear);
            }
            else
            {
                throw new InvalidOperationException("Friend list button must be childed by an AbsoluteLayout");
            }
        }

        private async Task closeFriendMenu()
        {
            await this.ScaleTo(0.8, 75, Easing.CubicOut);
            await this.ScaleTo(1, 75, Easing.CubicIn);
            
            m_FriendDisplay.TranslateTo(0, m_FriendDisplay.Height, 250, Easing.Linear);
            await m_MenuBackground.TranslateTo(0, m_FriendDisplay.Height, 250, Easing.Linear);

            ((AbsoluteLayout)this.Parent).Children.Remove(m_FriendDisplay);
            ((AbsoluteLayout)this.Parent).Children.Remove(m_MenuBackground);
            m_FriendDisplay = null;
            m_MenuBackground = null;
        }
    }
}
