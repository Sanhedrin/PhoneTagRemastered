using PhoneTag.XamarinForms.Controls.CameraControl;
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

        private void initializeComponent()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            buttonShoot = generateShootButton();

            Title = "PhoneTag!";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new RelativeLayout
            {
                Children = {
                    {
                        new StackLayout
                        {
                            VerticalOptions = new LayoutOptions {
                                Alignment = LayoutAlignment.Fill
                            },
                            Children = {
                                new RelativeLayout {
                                    Children = {
                                        {
                                            m_Camera = new CameraPreview {
                                                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                                                WidthRequest = CrossScreen.Current.Size.Width,
                                                Camera = CameraOptions.Rear,
                                                HorizontalOptions = LayoutOptions.Fill,
                                                VerticalOptions = LayoutOptions.Fill
                                            },
                                            Constraint.RelativeToParent((parent) => { return 0; })
                                        },
                                        {
                                            new Image {
                                                Aspect = Aspect.Fill,
                                                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                                                WidthRequest = CrossScreen.Current.Size.Width,
                                                Source = ImageSource.FromResource("PhoneTag.XamarinForms.Images.Crosshair.png")
                                            },
                                            Constraint.RelativeToParent((parent) => { return 0; })
                                        }
                                    }
                                },
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
                        },
                        Constraint.RelativeToParent((parent) => { return 0; })
                    }
                }
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
    }
}
