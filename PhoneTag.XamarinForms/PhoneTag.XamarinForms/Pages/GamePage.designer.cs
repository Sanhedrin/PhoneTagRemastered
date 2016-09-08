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
        private Grid m_GameLayout;
        private RelativeLayout m_CameraComponent;
        private Label m_TimerLabel;
        private bool m_IsBamming;

        private void initializeComponent()
        {
            BackgroundColor = Color.Black;

            NavigationPage.SetHasNavigationBar(this, false);

            buttonShoot = generateShootButton();
            Grid gameStackLayout = generateGameLayout();

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

            m_TimerLabel = generateTimerLabel();
            AbsoluteLayout.SetLayoutFlags(m_TimerLabel, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(m_TimerLabel, new Rectangle(10, 10, 0.3, 0.05));

            (Content as AbsoluteLayout).Children.Add(gameLayout);
            (Content as AbsoluteLayout).Children.Add(m_TimerLabel);

            initializeChat();
        }

        private Label generateTimerLabel()
        {
            m_TimerLabel = new Label()
            {
                BackgroundColor = Color.Transparent,
                TextColor = Color.White
            };

            startTimerCountDown(m_GameRoomView.GameDetails.GameDurationInMins);

            return m_TimerLabel;
        }

        private async Task startTimerCountDown(int i_GameDurationInMins)
        {
            TimeSpan timeLeft = new TimeSpan(i_GameDurationInMins / 60, i_GameDurationInMins % 60, 0);

            m_TimerLabel.Text = timeLeft.ToString();

            while (!m_GameOver && !timeLeft.Equals(new TimeSpan(0, 0, 0)))
            {
                timeLeft = timeLeft.Subtract(new TimeSpan(0, 0, 1));

                m_TimerLabel.Text = timeLeft.ToString();

                await Task.Delay(1000);
            }

            if (!m_GameOver)
            {
                m_GameRoomView.TimeUp();
            }
        }

        private Grid generateGameLayout()
        {
            m_CameraComponent = generateCameraComponent();

            m_GameLayout = new Grid
            {
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center },

                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(0.5, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(0.5, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(0.05, GridUnitType.Auto) }
                }
            };

            m_GameLayout.Children.Add(m_CameraComponent, 0, 0);
            m_GameLayout.Children.Add(m_GameMap, 0, 1);
            m_GameLayout.Children.Add(buttonShoot, 0, 2);

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
                Source = m_HaveLost ? "exit_button.png" : "shoot_button.png",
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
                yConstraint: Constraint.RelativeToParent((parent) => { return parentHeight(parent); }),
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }));

            await i_Dialog.TranslateTo((Width - i_Dialog.Width) / 2, -i_Dialog.Height, 750, Easing.SpringOut);
        }

        private double parentHeight(View parent)
        {
            return parent.Height;
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

        //Shows a quick BAM animation to show that the player was killed.
        private async Task bamTheScreen()
        {
            if (!m_IsBamming)
            {
                m_IsBamming = true;

                Image bamImage = new Image() { Source = "bam.png" };
                AbsoluteLayout.SetLayoutFlags(bamImage, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(bamImage, new Rectangle(0, 0, 1, 0.5));

                (Content as AbsoluteLayout).Children.Add(bamImage);

                //DependencyService.Get<ISound>().PlayBam();

                await bamImage.ScaleTo(0.001, 1, Easing.Linear);
                await bamImage.ScaleTo(1, 250, Easing.Linear);

                await Task.Delay(2000);

                (Content as AbsoluteLayout).Children.Remove(bamImage);
            }
        }

        private async Task transitionToSpectatorMode()
        {
            if (!m_HaveLost)
            {
                buttonShoot.Source = "exit_button.png";
                m_HaveLost = true;
                buttonShoot.IsEnabled = true;

                bamTheScreen();

                await m_CameraComponent.FadeTo(0, 750, Easing.Linear);

                if (!m_HaveLost)
                {
                    Label deadLabel = generateDeadLabel();

                    m_GameLayout.Children.Remove(m_CameraComponent);
                    m_GameLayout.Children.Insert(0, deadLabel);

                    deadLabel.FadeTo(1, 750, Easing.Linear);
                }
            }
        }

        private async Task transitionToGameEnd(List<String> i_WinnerIds)
        {
            m_HaveLost = true;
            buttonShoot.Source = "exit_button.png";
            buttonShoot.IsEnabled = true;

            m_TimerLabel.Text = String.Empty;

            await m_CameraComponent.FadeTo(0, 750, Easing.Linear);
            
            Label deadLabel = await generateGameEndLabel(i_WinnerIds);

            m_GameLayout.Children.RemoveAt(0);
            m_GameLayout.Children.Insert(0, deadLabel);

            deadLabel.FadeTo(1, 750, Easing.Linear);
        }

        private async Task<Label> generateGameEndLabel(List<String> i_WinnerIds)
        {
            StringBuilder gameEndMessage;

            if (i_WinnerIds.Count > 0)
            {
                gameEndMessage = new StringBuilder($"Game Over!.{Environment.NewLine}The winners are:{Environment.NewLine}");

                foreach (String id in i_WinnerIds)
                {
                    UserView user = await UserView.GetUser(id);
                    gameEndMessage.AppendLine(user.Username);
                }
            }
            else
            {
                gameEndMessage = new StringBuilder($"Game Over!.{Environment.NewLine}It's a tie!");
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
