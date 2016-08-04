using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class GameNotification : StackLayout
    {
        private const int k_DisplayTime = 3000;

        private void initializeComponent(String i_NotificationMessage)
        {
            Opacity = 0.6;
            HeightRequest = CrossScreen.Current.Size.Height / 16;

            Label messageLabel = generateMessageLabel(i_NotificationMessage);

            Children.Add(messageLabel);
        }

        private Label generateMessageLabel(String i_NotificationMessage)
        {
            Label messageLabel = new Label()
            {
                Text = i_NotificationMessage,
                TextColor = Color.White
            };

            return messageLabel;
        }

        /// <summary>
        /// Slides the notification into view.
        /// </summary>
        public async Task SlideIn()
        {
            RelativeLayout parentLayout = Parent as RelativeLayout;

            if (parentLayout != null)
            {
                TranslationX = TranslationY = 0;

                pushDownOlderMessages(parentLayout);

                await this.TranslateTo(-Width, 0, 750, Easing.CubicInOut);

                await Task.Delay(k_DisplayTime);

                removeMessage(parentLayout);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine("Can't slide in an UI element that isn't childed by a RelativeLayout");
                throw new InvalidOperationException("Can't slide in an UI element that isn't childed by a RelativeLayout");
            }
        }

        //Removes the notification from the ui.
        private async Task removeMessage(RelativeLayout i_ParentLayout)
        {
            await this.FadeTo(0, 1500, Easing.Linear);

            i_ParentLayout.Children.Remove(this);
        }

        //Pushes all current notifications down to make room for a now one.
        private void pushDownOlderMessages(RelativeLayout i_ParentLayout)
        {
            foreach (View child in i_ParentLayout.Children)
            {
                GameNotification notification = child as GameNotification;

                if (notification != null)
                {
                    notification.SlideDown();
                }
            }
        }

        //Slides the current notification down to make room for a new one.
        private async Task SlideDown()
        {
            await this.TranslateTo(0, Height, 750, Easing.CubicInOut);
        }
    }
}
