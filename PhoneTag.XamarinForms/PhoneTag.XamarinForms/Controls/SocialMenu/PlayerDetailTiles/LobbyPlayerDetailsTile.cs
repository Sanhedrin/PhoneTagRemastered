using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.MenuButtons;
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
        private ImageButton m_SocialButton;
        private Image m_ReadyImage;

        public LobbyPlayerDetailsTile(UserView i_UserView) : base(i_UserView)
        {
            setupTile();
        }

        protected override void setupTile()
        {
            Grid detailsGrid = generateTileLayout();

            Children.Add(detailsGrid, 
                Constraint.RelativeToParent((parent) => { return parent.Width / 8; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 5; }));

            initializeComponent();
        }

        private Grid generateTileLayout()
        {
            Grid layout = new Grid()
            {
                ColumnSpacing = 10,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) }
                }
            };
            
            m_ReadyImage = generateReadyImage();
            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();
            //m_SocialButton = generateSocialOperationButton();

            layout.Children.Add(m_ReadyImage, 0, 0);
            layout.Children.Add(profilePic, 1, 0);
            layout.Children.Add(nameLabel, 2, 0);
            //layout.Children.Add(m_SocialButton, 3, 0);

            return layout;
        }

        //Returns a button that shows whether this player is your friend or lets you add them as a friend.
        private ImageButton generateSocialOperationButton()
        {
            ImageButton socialButton = new ImageButton();

            socialButton.Aspect = Aspect.AspectFill;

            VerticalOptions = HorizontalOptions = new LayoutOptions
            {
                Alignment = LayoutAlignment.Fill
            };

            bool isFriended = UserView.Current.Friends.Exists(user => user.FBID.Equals(UserView.FBID));
            bool isMe = UserView.Current.FBID.Equals(UserView.FBID);

            //Show the add friend button if relevant.
            if (!isMe && !isFriended)
            {
                socialButton.Source = "add_friend_button.png";

                socialButton.ClickAction = () => { addFriend(); };
            }

            return socialButton;
        }

        private async Task addFriend()
        {
            m_SocialButton.IsEnabled = false;

            await UserView.Current.AddFriend(UserView.FBID);

            m_SocialButton.Source = null;
            m_SocialButton.ClickAction = null;
        }

        //Returns a box that's colored according to the player's ready status.
        private Image generateReadyImage()
        {
            Image readyImage = new Image() { Aspect = Aspect.AspectFill };

            readyImage.Source = UserView.IsReady ? "ready_check.png" : "unready_check.png";

            return readyImage;
        }

        public override void Refresh(PlayerDetailsTile i_PlayerDetails)
        {
            m_ReadyImage.Source = i_PlayerDetails.UserView.IsReady ? "ready_check.png" : "unready_check.png";
        }
    }
}
