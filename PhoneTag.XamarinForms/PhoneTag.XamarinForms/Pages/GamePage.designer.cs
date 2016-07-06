using PhoneTag.XamarinForms.Controls;
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
    public partial class GamePage : ContentPage
    {
        private void initializeComponent()
        {
            Title = "PhoneTag!";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
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
                            new Button { Text = "Reload", BackgroundColor = Color.Green },
                            new Button {
                                Text = "Shoot", BackgroundColor = Color.Red,
                                Command = new Command( () => { GamePage_ShootButtonClicked(); })
                            }
                        }
                    }
                }
            };
        }
    }
}
