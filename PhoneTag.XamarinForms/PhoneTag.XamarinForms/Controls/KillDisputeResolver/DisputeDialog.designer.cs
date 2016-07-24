using PhoneTag.SharedCodebase.Events.GameEvents;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class DisputeDialog : StackLayout
    {
        private ProgressBar m_TimeoutBar;

        private void initializeComponent(KillDisputeEvent i_KillDisputeEvent)
        {
            BackgroundColor = Color.Black;
            Padding = new Thickness() { Top = 20, Bottom = 20 };
            WidthRequest = CrossScreen.Current.Size.Width * 15 / 16;
            HeightRequest = CrossScreen.Current.Size.Height / 10;

            HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };
            VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            m_TimeoutBar = generateTimeoutBar();
            Label disputeRequestLabel = generateDisputeComment();

            Children.Add(m_TimeoutBar);
            Children.Add(disputeRequestLabel);

            timeoutNotification();
        }

        //Counts down to remove the alert.
        private async Task timeoutNotification()
        {
            await m_TimeoutBar.ProgressTo(1, 30000, Easing.Linear);

            if(Timeout != null)
            {
                Timeout(this, new EventArgs());
            }
        }

        private Label generateDisputeComment()
        {
            Label label = new Label()
            {
                Text = "Pending kill dispute",
                HorizontalTextAlignment = TextAlignment.Center
            };

            return label;
        }

        private ProgressBar generateTimeoutBar()
        {
            m_TimeoutBar = new ProgressBar()
            {
                Progress = 0
            };

            return m_TimeoutBar;
        }
    }
}
