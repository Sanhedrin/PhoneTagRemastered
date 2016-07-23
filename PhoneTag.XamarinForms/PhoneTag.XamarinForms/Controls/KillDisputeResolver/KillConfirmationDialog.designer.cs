using PhoneTag.SharedCodebase;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Utils;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class KillConfirmationDialog : StackLayout
    {
        private async Task initializeComponent(KillRequestEvent i_KillRequest)
        {
            BackgroundColor = Color.Black;
            Padding = new Thickness() { Top = 20, Bottom = 20 };
            WidthRequest = CrossScreen.Current.Size.Width * 15 / 16;

            HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };
            VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            Image killcam = await generateKillCamImage(i_KillRequest.KillCamId);
            Label killComment = generateKillComment(i_KillRequest.RequestedBy);
            StackLayout responseButtons = generateResponseButtons();

            Children.Add(killcam);
            Children.Add(killComment);
            Children.Add(responseButtons);
        }

        private StackLayout generateResponseButtons()
        {
            StackLayout responseButtons = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center }
            };

            responseButtons.Children.Add(new Button()
            {
                Text = "Confirm",
                Command = new Command(() => {
                    if (KillConfirmed != null) KillConfirmed(this, new EventArgs());
                })
            });
            responseButtons.Children.Add(new Button()
            {
                Text = "Deny",
                Command = new Command(() => {
                    if (KillDenied != null) KillDenied(this, new EventArgs());
                })
            });

            return responseButtons;
        }

        private Label generateKillComment(string i_RequestedBy)
        {
            Label killComment = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = String.Format("You have been killed by {0}", i_RequestedBy)
            };

            return killComment; 
        }

        private async Task<Image> generateKillCamImage(String i_KillCamId)
        {
            Image killcam = new Image()
            {
                WidthRequest = CrossScreen.Current.Size.Width * 2 / 3,
                HeightRequest = CrossScreen.Current.Size.Height * 3 / 4
            };

            Uri killCamUri = new Uri($"{Keys.ImageHostingServiceDownloadUrl}/{i_KillCamId}.jpg");
            killcam.Source = ImageSource.FromUri(killCamUri);

            return killcam;
        }
    }
}
