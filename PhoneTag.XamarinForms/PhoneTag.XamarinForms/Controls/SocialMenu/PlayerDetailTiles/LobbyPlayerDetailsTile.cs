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
    public class LobbyPlayerDetailsTile : PlayerDetailsTile
    {
        private Image m_SocialButton;
        private BoxView m_ReadyBox;

        public LobbyPlayerDetailsTile(UserView i_UserView) : base(i_UserView)
        {
            setupTile();
        }

        protected override void setupTile()
        {
            StackLayout detailsStack = generateDetailStack();
            StackLayout actionStack = generateActionStack();
            
            Children.Add(detailsStack, 
                Constraint.RelativeToParent((parent) => { return 0; }),
                Constraint.RelativeToParent((parent) => { return CrossScreen.Current.Size.Height / 16; }));
            Children.Add(actionStack, 
                Constraint.RelativeToParent((parent) => { return CrossScreen.Current.Size.Width / 2; }),
                Constraint.RelativeToParent((parent) => { return CrossScreen.Current.Size.Height / 16; }));

            initializeComponent();
        }

        private StackLayout generateActionStack()
        {
            StackLayout layout = new StackLayout();

            m_SocialButton = generateSocialOperationButton();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            layout.Children.Add(m_SocialButton);

            return layout;
        }

        private StackLayout generateDetailStack()
        {
            StackLayout layout = new StackLayout();
            
            m_ReadyBox = generateReadyBox();
            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Start };
            layout.VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };

            layout.Children.Add(m_ReadyBox);
            layout.Children.Add(profilePic);
            layout.Children.Add(nameLabel);

            return layout;
        }

        //Returns a button that shows whether this player is your friend or lets you add them as a friend.
        private Image generateSocialOperationButton()
        {
            Image socialButton = new Image();

            socialButton.Aspect = Aspect.AspectFill;

            VerticalOptions = HorizontalOptions = new LayoutOptions
            {
                Alignment = LayoutAlignment.Fill
            };

            bool isFriended = UserView.Current.Friends.Exists(user => user.FBID.Equals(UserView.FBID));
            bool isMe = UserView.Current.FBID.Equals(UserView.FBID);

            //Show icon indicating that you're already friends.
            if (isFriended)
            {
                socialButton.Source = "friend_indication.png";
            }
            //Show add friend button if the player isn't me.
            else if(!isMe)
            {
                socialButton.Source = "add_friend_button.png";

                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                GestureRecognizers.Add(tapGestureRecognizer);
            }

            return socialButton;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            addFriend();
        }

        private async Task addFriend()
        {
            m_SocialButton.IsEnabled = false;
        }

        //Returns a box that's colored according to the player's ready status.
        private BoxView generateReadyBox()
        {
            BoxView readyBox = new BoxView();

            readyBox.Color = UserView.IsReady ? Color.Green : Color.Red;

            return readyBox;
        }

        public override void Refresh(PlayerDetailsTile i_PlayerDetails)
        {
            m_ReadyBox.Color = i_PlayerDetails.UserView.IsReady ? Color.Green : Color.Red;
        }
    }
}
