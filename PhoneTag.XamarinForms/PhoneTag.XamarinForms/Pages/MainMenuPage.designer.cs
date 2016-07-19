using PhoneTag.XamarinForms.Controls.FriendMenu;
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
            StackLayout layout = generatePageLayout();
            FriendListButton friendListButton = generateFriendListButton();

            Title = "Main Menu";
            Padding = new Thickness(0, 20, 0, 0);
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
            FriendListButton friendsButton = new FriendListButton()
            {
            };

            return friendsButton;
        }

        private StackLayout generatePageLayout()
        {
            StackLayout layout = new StackLayout
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
                        //TODO: Insert title image asset.
                        new Button()
                        {
                            Text = "Find Game In Your Area!",
                            BackgroundColor = Color.Yellow,
                            Command = new Command(() => { FindGameButton_Clicked(); })
                        },
                        new Button()
                        {
                            Text = "Create Game!",
                            BackgroundColor = Color.Green,
                            Command = new Command(() => { CreateGameButton_Clicked(); })
                        },
                        //TODO: Add friend list component here.
                    }
            };

            AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(1f, 1f, 1f, 1f));
            AbsoluteLayout.SetLayoutFlags(layout, AbsoluteLayoutFlags.All);

            return layout;
        }
    }
}
