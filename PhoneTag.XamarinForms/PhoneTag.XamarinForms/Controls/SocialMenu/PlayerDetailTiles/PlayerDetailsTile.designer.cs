using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    public abstract partial class PlayerDetailsTile : RelativeLayout
    {
        //Initializes common components of the tile.
        protected void initializeComponent()
        {
            BackgroundColor = Color.White;
            Padding = 10;

            HorizontalOptions = new LayoutOptions()
            {
                Alignment = LayoutAlignment.Fill
            };
            
            HeightRequest = CrossScreen.Current.Size.Height / 8;
        }

        //Gets the profile picture of the current user.
        protected Image generateProfilePicture()
        {
            Image profilePicture = new Image();

            profilePicture.Source = ImageSource.FromUri(new Uri(m_UserView.ProfilePicUrl));

            return profilePicture;
        }

        //Gets the name of the current user.
        protected Label generateUserNameLabel()
        {
            Label nameLabel = new Label();

            nameLabel.TextColor = Color.Black;
            nameLabel.Text = m_UserView.Username.Substring(0, Math.Min(m_UserView.Username.IndexOf(" "), m_UserView.Username.Length));

            return nameLabel;
        }
    }
}
