using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.Controls.MenuButtons;
using PhoneTag.XamarinForms.Controls.SocialMenu;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class MainMenuPage: TrailableContentPage
    {
        private void initializeComponent()
        {
            RelativeLayout layout = generatePageLayout();
            FriendListButton friendListButton = generateFriendListButton();

            BackgroundImage = "mainmenu_background.png";
            Title = "Main Menu";
            Padding = new Thickness(0, 20, 0, 0);
            BackgroundColor = Color.White;
            Content = new AbsoluteLayout
            {
                Children = {
                    layout,
                    friendListButton
                }
            };
        }

        private FriendListButton generateFriendListButton()
        {
            FriendListButton friendsButton = new FriendListButton();

            return friendsButton;
        }

        private RelativeLayout generatePageLayout()
        {
            RelativeLayout layout = new RelativeLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                HorizontalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    {
                        new ImageButton()
                        {
                            Source = new FileImageSource() {
                                File = "create_button.png"
                            },
                            ClickAction = () => { CreateGameButton_Clicked(); }
                        },
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.170; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.22; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.677; })
                    },
                    {
                        new ImageButton()
                        {
                            Source = new FileImageSource() {
                                File = "search_button.png"
                            },
                            ClickAction = () => { FindGameButton_Clicked(); }
                        },
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.170; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.661; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.677; })

                    },
                    {
                        new ImageButton()
                        {
                            Source = new FileImageSource() {
                                File = "settings_button.png"
                            },
                            ClickAction = () => { SettingsButton_Clicked(); }
                        },
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.05; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.68; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.15; })
                    }
                }
            };

            AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(1f, 1f, 1f, 1f));
            AbsoluteLayout.SetLayoutFlags(layout, AbsoluteLayoutFlags.All);

            return layout;
        }
    }
}
