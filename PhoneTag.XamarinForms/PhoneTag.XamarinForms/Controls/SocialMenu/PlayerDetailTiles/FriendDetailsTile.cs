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
            StackLayout detailsStack = generateDetailStack();
            StackLayout actionStack = generateActionStack();

            Children.Add(detailsStack,
                Constraint.RelativeToParent((parent) => { return 0; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 4; }),
                Constraint.RelativeToParent((parent) => { return parent.Width / 3; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 2; }));
            Children.Add(actionStack,
                Constraint.RelativeToParent((parent) => { return parent.Width / 3; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 4; }),
                Constraint.RelativeToParent((parent) => { return parent.Width * 2 / 3; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 2; }));

            initializeComponent();
        }

        private StackLayout generateDetailStack()
        {
            StackLayout verticalLayout = new StackLayout() { Orientation = StackOrientation.Vertical };

            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();

            verticalLayout.Orientation = StackOrientation.Horizontal;
            verticalLayout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Start };
            verticalLayout.VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };

            verticalLayout.Children.Add(profilePic);
            verticalLayout.Children.Add(nameLabel);

            return verticalLayout;
        }

        private StackLayout generateActionStack()
        {
            StackLayout layout = new StackLayout();

            View chatButton = generateChatButton();
            //View gameOperationButton = generateGameOperationButton();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            //layout.Children.Add(gameOperationButton);
            layout.Children.Add(chatButton);

            return layout;
        }

        //Creates a button that lets you join the game of the current user if they're in a game and you're not
        //or invite them if you're in a game and they're not.
        private Button generateGameOperationButton()
        {
            Button gameOperationButton = new Button();

            bool amIInGame = !String.IsNullOrEmpty(UserView.Current.PlayingIn);
            bool areTheyInGame = !String.IsNullOrEmpty(UserView.PlayingIn);

            //Invite button
            if (amIInGame && !areTheyInGame)
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
            //TODO: Add updating to friend's activity.
            throw new NotImplementedException();
        }
    }
}
