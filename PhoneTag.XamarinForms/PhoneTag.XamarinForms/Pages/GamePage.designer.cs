using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.CameraControl;
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GamePage : TrailableContentPage
    {
        private Stack<View> m_CurrentlyShowingDialogs = new Stack<View>();

        private Button buttonShoot;
        private StackLayout m_GameLayout;
        private RelativeLayout m_CameraComponent;

        private void initializeComponent()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            buttonShoot = generateShootButton();
            StackLayout gameStackLayout = generateGameLayout();

            Title = "PhoneTag!";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new RelativeLayout
            {
                Children = {
                    {
                        gameStackLayout,
                        Constraint.RelativeToParent((parent) => { return 0; })
                    }
                }
            };
        }

        private StackLayout generateGameLayout()
        {
            m_CameraComponent = generateCameraComponent();

            m_GameLayout = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    m_CameraComponent,
                    new RelativeLayout {
                        Children = {
                            {
                                m_GameMap,
                                Constraint.RelativeToParent((parent) => { return 0; })
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = new LayoutOptions
                        {
                            Alignment = LayoutAlignment.Center,
                            Expands = true
                        },
                        Children = {
                            buttonShoot
                        }
                    }
                }
            };

            return m_GameLayout;
        }

        private RelativeLayout generateCameraComponent()
        {
            Image crosshairImage = generateCrosshairImage();
            m_Camera = generateCameraStreamRenderer();

            RelativeLayout cameraComponent = new RelativeLayout
            {
                Children = {
                    {
                        m_Camera,
                        Constraint.RelativeToParent((parent) => { return 0; })
                    },
                    {
                        crosshairImage,
                        Constraint.RelativeToParent((parent) => { return 0; })
                    }
                }
            };

            return cameraComponent;
        }

        private CameraPreview generateCameraStreamRenderer()
        {
            CameraPreview cameraPreview = new CameraPreview
            {
                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                WidthRequest = CrossScreen.Current.Size.Width,
                Camera = CameraOptions.Rear,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            return cameraPreview;
        }

        private Image generateCrosshairImage()
        {
            return new Image
            {
                Aspect = Aspect.Fill,
                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                WidthRequest = CrossScreen.Current.Size.Width,
                Source = ImageSource.FromResource("PhoneTag.XamarinForms.Images.Crosshair.png")
            };
        }

        private Button generateShootButton()
        {
            Button shootButton = new Button
            {
                Text = "Shoot!",
                BackgroundColor = Color.Red,
            };

            shootButton.Clicked += ShootButton_Clicked;

            return shootButton;
        }

        //Displays a notification saying that the player died.
        private async Task displayNotification(PlayerKilledEvent i_PlayerKilledEvent)
        {
            if (i_PlayerKilledEvent != null && !String.IsNullOrEmpty(i_PlayerKilledEvent.PlayerFBID))
            {
                UserView user = await UserView.GetUser(i_PlayerKilledEvent.PlayerFBID);

                if (user != null)
                {
                    GameNotification notification = new GameNotification($"{user.Username} was killed!");

                    RelativeLayout contentLayout = Content as RelativeLayout;

                    if(contentLayout != null)
                    {
                        contentLayout.Children.Add(notification,
                            xConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                            yConstraint: Constraint.RelativeToParent((parent) => { return 0; }));

                        notification.SlideIn();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Can't use a game notification on a non-relative layout");
                    }
                }
            }
        }

        private async Task showDialog(View i_Dialog)
        {
            m_CurrentlyShowingDialogs.Push(i_Dialog);
            i_Dialog.TranslationX = i_Dialog.TranslationY = 0;

            (Content as RelativeLayout).Children.Add(i_Dialog, 
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));
            
            i_Dialog.TranslateTo((Width - i_Dialog.Width) / 2, -i_Dialog.Height, 750, Easing.SpringOut);
        }

        private async Task hideDialog()
        {
            View dialog = m_CurrentlyShowingDialogs.Pop();

            await dialog.TranslateTo(0, Height, 750, Easing.SpringIn);

            (Content as RelativeLayout).Children.Remove(dialog);

            buttonShoot.Text = "Shoot!";
            buttonShoot.IsEnabled = true;
        }

        private async Task transitionToSpectatorMode()
        {
            buttonShoot.Text = "Quit";
            buttonShoot.IsEnabled = true;

            await m_CameraComponent.FadeTo(0, 750, Easing.Linear);

            Label deadLabel = generateDeadLabel();

            m_GameLayout.Children.Remove(m_CameraComponent);
            m_GameLayout.Children.Insert(0, deadLabel);

            deadLabel.FadeTo(1, 750, Easing.Linear);
        }

        private Label generateDeadLabel()
        {
            Label deadLabel = new Label()
            {
                Text = $"You are dead.{Environment.NewLine}You can leave the game or keep spectating.",
                HeightRequest = m_Camera.Height,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Opacity = 0
            };

            return deadLabel;
        }
    }
}
