using PhoneTag.SharedCodebase.Views;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    /// <summary>
    /// Displays a UI element detailing the given user as part of a friend list UI.
    /// </summary>
    public class FriendDetailsTile : PlayerDetailsTile
    {
        public FriendDetailsTile(UserView i_UserView) : base(i_UserView)
        {
            setupTile();
        }

        protected override void setupTile()
        {
            Grid detailsGrid = generateDetailGrid();

            Children.Add(detailsGrid,
                Constraint.RelativeToParent((parent) => { return parent.Width / 8; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 5; }));

            initializeComponent();
        }

        private Grid generateDetailGrid()
        {
            Grid verticalLayout = new Grid()
            { 
                ColumnSpacing = 10,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) }
                }
            };

            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();
            
            verticalLayout.Children.Add(profilePic, 0, 0);
            verticalLayout.Children.Add(nameLabel, 1, 0);

            return verticalLayout;
        }

        private StackLayout generateActionStack()
        {
            StackLayout layout = new StackLayout();

            //View chatButton = generateChatButton();
            //View gameOperationButton = generateGameOperationButton();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            //layout.Children.Add(gameOperationButton);
            //layout.Children.Add(chatButton);

            return layout;
        }

        //Creates a button that lets you join the game of the current user if they're in a game and you're not
        //or invite them if you're in a game and they're not.
        private Button generateGameOperationButton()
        {
            Button gameOperationButton = new Button();

            bool amIInGame = !String.IsNullOrEmpty(UserView.Current.PlayingIn);
            bool areTheyInGame = !String.IsNullOrEmpty(UserView.PlayingIn);

            //Offline
            if (!UserView.IsActive)
            {
                gameOperationButton.Text = "Offline";
                gameOperationButton.TextColor = Color.Black;
                gameOperationButton.IsEnabled = false;
            }
            //Invite button
            else if (amIInGame && !areTheyInGame)
            {
                gameOperationButton.Text = "Invite";
                gameOperationButton.TextColor = Color.Black;
                gameOperationButton.IsEnabled = true;
            }
            //Join button
            else if(areTheyInGame && !amIInGame)
            {
                gameOperationButton.Text = "Invite";
                gameOperationButton.TextColor = Color.Black;
                gameOperationButton.IsEnabled = true;
            }
            //Both in game.
            else if(amIInGame && areTheyInGame)
            {
                gameOperationButton.Text = "In Game";
                gameOperationButton.TextColor = Color.Black;
                gameOperationButton.IsEnabled = false;
            }
            //Both in menus
            else if (!amIInGame && !areTheyInGame)
            {
                gameOperationButton.Text = "In Menus";
                gameOperationButton.TextColor = Color.Black;
                gameOperationButton.IsEnabled = false;
            }

            return gameOperationButton;
        }

        //Returns a button that starts a chat with the given user.
        private Button generateChatButton()
        {
            Button chatButton = new Button();

            chatButton.Text = "Chat";
            chatButton.TextColor = Color.Black;
            chatButton.IsEnabled = false;

            return chatButton;
        }

        public override void Refresh(PlayerDetailsTile i_PlayerDetails)
        {
            //Nothing to update on friend tile.
        }
    }
}
