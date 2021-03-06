﻿using PhoneTag.SharedCodebase.Utils;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class NotificationDialog : StackLayout
    {
        private ProgressBar m_TimeoutBar;

        private void initializeComponent(String i_Message)
        {
            BackgroundColor = Color.Black;
            Opacity = 0.8;
            Padding = new Thickness() { Top = 0, Bottom = 20 };
            WidthRequest = CrossScreen.Current.Size.Width * 15 / 16;
            HeightRequest = CrossScreen.Current.Size.Height / 10;

            HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };
            VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            m_TimeoutBar = generateTimeoutBar();
            Label disputeRequestLabel = generateNotificationComment(i_Message);

            Children.Add(disputeRequestLabel);
            Children.Add(m_TimeoutBar);

            timeoutNotification();
        }

        //Counts down to remove the alert.
        private async Task timeoutNotification()
        {
            await m_TimeoutBar.ProgressTo(1, Keys.DisputeTimeInSeconds * 1000, Easing.Linear);

            if (Timeout != null)
            {
                Timeout(this, new EventArgs());
            }
        }

        private Label generateNotificationComment(String i_Message)
        {
            Label label = new Label()
            {
                Text = i_Message,
                TextColor = Color.White,
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
