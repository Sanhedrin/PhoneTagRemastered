using PhoneTag.SharedCodebase;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.POCOs;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.Pages;
using Plugin.Settings;
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
        private StackLayout m_LoadingLayout;
        private Button m_DenyButton;
        private Button m_ConfirmButton;

        private KillDisputeEventArgs m_KillRequest;

        private async Task initializeComponent(String i_DisputeId, KillRequestEvent i_KillRequest, String i_DisplayMessage)
        {
            bool displayTip = CrossSettings.Current.GetValueOrDefault("KillConfirmations", true);

            if (displayTip)
            {
                bool understood = await TrailableContentPage.CurrentPage.DisplayAlert("Kill Confirmations",
                    $"When a player shoots you, you will be shown the shot they've taken of you, and be given a chance to confirm or deny the shot, this is done to prevent cheating.{Environment.NewLine}If the shot captured you in it, press the confirm button.{Environment.NewLine}If you don't think you were rightfully killed, press the deny button;{Environment.NewLine}Denying will send a dispute to the rest of the players, showing the shot image and letting everyone vote on the result. If the majory of players vote to keep you alive, the attacker will be killed instead as penalty.", "Don't show again", "Ok");

                CrossSettings.Current.AddOrUpdateValue("KillConfirmations", !understood);
            }

            BackgroundColor = Color.Black;
            Padding = new Thickness() { Top = 20, Bottom = 20 };
            WidthRequest = CrossScreen.Current.Size.Width * 15 / 16;

            HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };
            VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            Label killComment = generateKillComment(i_DisplayMessage);
            StackLayout responseButtons = await generateResponseButtons(i_DisputeId, i_KillRequest);

            if (String.IsNullOrEmpty(i_KillRequest.KillCamId))
            {
                m_LoadingLayout = generateLoadingLayout();
                Children.Add(m_LoadingLayout);
            }
            else
            {
                Image killcam = await generateKillCamImage(i_KillRequest.KillCamId);
                Children.Add(killcam);
            }

            Children.Add(killComment);
            Children.Add(responseButtons);
        }

        private StackLayout generateLoadingLayout()
        {
            return new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new AnimatedImage()
                    {
                        ImageName = "loading_logo",
                        Animate = true,
                        AnimationFrames = 30
                    },
                    new Label
                    {
                        Text = "Loading...",
                        TextColor = Color.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
        }

        private async Task<StackLayout> generateResponseButtons(String i_DisputeId, KillRequestEvent i_KillRequestDetails)
        {
            StackLayout responseButtons = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center }
            };
            
            m_KillRequest = new KillDisputeEventArgs(i_DisputeId, i_KillRequestDetails.RoomId,
                        UserView.Current.FBID, i_KillRequestDetails.RequestedById, UserView.Current.Username, 
                        i_KillRequestDetails.RequestedBy, i_KillRequestDetails.KillCamId);

            m_ConfirmButton = new Button()
            {
                Text = "Confirm",
                IsEnabled = !String.IsNullOrEmpty(i_KillRequestDetails.KillCamId),
                BackgroundColor = Color.Silver,
                TextColor = Color.Black
            };
            m_ConfirmButton.Clicked += ConfirmButton_Clicked;

            m_DenyButton = new Button()
            {
                Text = "Deny",
                IsEnabled = !String.IsNullOrEmpty(i_KillRequestDetails.KillCamId),
                BackgroundColor = Color.Silver,
                TextColor = Color.Black
            };
            m_DenyButton.Clicked += DenyButton_Clicked;

            responseButtons.Children.Add(m_ConfirmButton);
            responseButtons.Children.Add(m_DenyButton);

            return responseButtons;
        }

        public async Task AttachImage(string i_KillCamId)
        {
            m_KillRequest.KillCamId = i_KillCamId;

            Children.Remove(m_LoadingLayout);

            m_DenyButton.IsEnabled = true;
            m_ConfirmButton.IsEnabled = true;

            Image killcam = await generateKillCamImage(i_KillCamId);
            Children.Insert(0, killcam);
        }

        private Label generateKillComment(string i_Message)
        {
            Label killComment = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = i_Message,
                TextColor = Color.White
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
