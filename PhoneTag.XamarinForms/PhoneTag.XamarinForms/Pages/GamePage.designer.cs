using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.CameraControl;
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
using PhoneTag.XamarinForms.Controls.MenuButtons;
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
    public partial class GamePage : ChatEmbeddedContentPage
    {
        private Stack<View> m_CurrentlyShowingDialogs = new Stack<View>();

        private ImageButton buttonShoot;
        private StackLayout m_GameLayout;
        private RelativeLayout m_CameraComponent;
        private bool m_GameOver = false;

        private void initializeComponent()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            buttonShoot = generateShootButton();
            StackLayout gameStackLayout = generateGameLayout();

            Title = "PhoneTag!";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new AbsoluteLayout();
            
            RelativeLayout gameLayout = new RelativeLayout
            {
                Children = {
                    {
                        gameStackLayout,
                        Constraint.RelativeToParent((parent) => { return 0; })
                    }
                }
            };
            AbsoluteLayout.SetLayoutFlags(gameLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(gameLayout, new Rectangle(0, 0, 1, 1));

            (Content as AbsoluteLayout).Children.Add(gameLayout);

            initializeChat();
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
                                Constraint.RelativeToParent((parent) => { return 0; }),
                                Constraint.RelativeToParent((parent) => { return 0; }),
                                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                                Constraint.RelativeToParent((parent) => { return parent.Height * 0.65; })
                            },
                            {
                                buttonShoot,
                                Constraint.RelativeToParent((parent) => { return parent.Width * 0.4; }),
                                Constraint.RelativeToParent((parent) => { return parent.Height * 0.425; }),
                                Constraint.RelativeToParent((parent) => { return parent.Width * 0.2125; })
                            }
                        }
                    },
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

        private ImageButton generateShootButton()
        {
            ImageButton shootButton = new ImageButton
            {
                Source = "shoot_button.png",
                IsEnabled = true,
                ClickAction = () => { ShootButton_Clicked(); }
            };

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

                    RelativeLayout contentLayout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);
                    
                    if (contentLayout != null)
                    {
                        contentLayout?.Children.Add(notification,
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

        private async Task showDialogSlideDown(View i_Dialog)
        {
            m_CurrentlyShowingDialogs.Push(i_Dialog);
            i_Dialog.TranslationX = i_Dialog.TranslationY = 0;

            RelativeLayout layout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);

            layout?.Children.Add(i_Dialog, 
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return 0; }));

            await i_Dialog.TranslateTo(0, -i_Dialog.Height, 1, null);
            await i_Dialog.TranslateTo((Width - i_Dialog.Width) / 2, 0, 750, Easing.SpringOut);
        }

        private async Task showDialogSlideUp(View i_Dialog)
        {
            m_CurrentlyShowingDialogs.Push(i_Dialog);
            i_Dialog.TranslationX = i_Dialog.TranslationY = 0;

            RelativeLayout layout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);

            layout?.Children.Add(i_Dialog,
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));
            
            await i_Dialog.TranslateTo((Width - i_Dialog.Width) / 2, -i_Dialog.Height, 750, Easing.SpringOut);
        }

        private async Task finishShowDialogSlideUp(View i_Dialog)
        {
            RelativeLayout layout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);

            layout?.Children.Add(i_Dialog,
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));

            await i_Dialog.TranslateTo((Width - i_Dialog.Width) / 2, -i_Dialog.Height, 750, Easing.SpringOut);
        }

        private async Task hideDialogSlideUp()
        {
            if (m_CurrentlyShowingDialogs.Count > 0)
            {
                View dialog = m_CurrentlyShowingDialogs.Pop();

                await dialog.TranslateTo(0, -dialog.Height, 750, Easing.SpringIn);
                
                RelativeLayout layout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);

                layout?.Children.Remove(dialog);
                
                buttonShoot.IsEnabled = true;
            }
        }

        private async Task hideDialogSlideDown()
        {
            if (m_CurrentlyShowingDialogs.Count > 0)
            {
                View dialog = m_CurrentlyShowingDialogs.Pop();

                await dialog.TranslateTo(0, Height, 750, Easing.SpringIn);

                RelativeLayout layout = ((Content as AbsoluteLayout).Children.Where((view) => view is RelativeLayout).First() as RelativeLayout);

                layout?.Children.Remove(dialog);
                
                buttonShoot.IsEnabled = true;
            }
        }

        private async Task transitionToSpectatorMode()
        {
            if (!m_GameOver)
            {
                buttonShoot.Source = "exit_button.png";
                m_GameOver = true;
                buttonShoot.IsEnabled = true;

                await m_CameraComponent.FadeTo(0, 750, Easing.Linear);

                if (!m_GameOver)
                {
                    Label deadLabel = generateDeadLabel();

                    m_GameLayout.Children.Remove(m_CameraComponent);
                    m_GameLayout.Children.Insert(0, deadLabel);

                    buttonShoot.TranslateTo(0, Height / 12, 1, null);

                    deadLabel.FadeTo(1, 750, Easing.Linear);
                }
            }
        }

        private async Task transitionToGameEnd(List<String> i_WinnerIds)
        {
            m_GameOver = true;
            buttonShoot.Source = "exit_button.png";
            buttonShoot.IsEnabled = true;

            await m_CameraComponent.FadeTo(0, 750, Easing.Linear);
            
            Label deadLabel = await generateGameEndLabel(i_WinnerIds);

            m_GameLayout.Children.RemoveAt(0);
            m_GameLayout.Children.Insert(0, deadLabel);

            buttonShoot.TranslateTo(0, Height / 12, 1, null);

            deadLabel.FadeTo(1, 750, Easing.Linear);
        }

        private async Task<Label> generateGameEndLabel(List<String> i_WinnerIds)
        {
            StringBuilder gameEndMessage = new StringBuilder($"Game Over!.{Environment.NewLine}The winners are:{Environment.NewLine}");

            foreach(String id in i_WinnerIds)
            {
                UserView user = await UserView.GetUser(id);
                gameEndMessage.AppendLine(user.Username);
            }

            Label deadLabel = new Label()
            {
                Text = gameEndMessage.ToString(),
                TextColor = Color.White,
                HeightRequest = m_Camera.Height / 2,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Opacity = 0
            };

            return deadLabel;
        }

        private Label generateDeadLabel()
        {
            Label deadLabel = new Label()
            {
                Text = $"You are dead.{Environment.NewLine}You can leave the game or keep spectating.",
                TextColor = Color.White,
                HeightRequest = m_Camera.Height / 2,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Opacity = 0
            };

            return deadLabel;
        }
    }
}
