﻿using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
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
        private FriendListButton m_FriendsButton;

        private void initializeComponent()
        {
            RelativeLayout layout = generatePageLayout();
            FriendListButton friendListButton = generateFriendListButton();

            Image bgImage = new Image
            {
                Source = "mainmenu_background.png",
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutFlags(bgImage, AbsoluteLayoutFlags.None);
            AbsoluteLayout.SetLayoutBounds(bgImage, new Rectangle(0, 0, CrossScreen.Current.Size.Width, CrossScreen.Current.Size.Height));

            Title = "Main Menu";
            BackgroundColor = Color.White;
            Content = new AbsoluteLayout
            {
                Children = {
                    bgImage,
                    layout,
                    friendListButton
                }
            };
        }

        private FriendListButton generateFriendListButton()
        {
            m_FriendsButton = new FriendListButton();

            return m_FriendsButton;
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
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.28; }),
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
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.761; }),
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
