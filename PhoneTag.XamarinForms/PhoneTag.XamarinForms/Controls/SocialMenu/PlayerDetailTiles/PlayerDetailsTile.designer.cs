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
            BackgroundColor = Color.FromRgba(210f / 255f, 210f / 255f, 210f / 255f, 255f / 255f);
            Children.Add(new Image { Source = "killcam_frame.png", Aspect = Aspect.Fill },
                Constraint.RelativeToParent((parent) => { return 0; }),
                Constraint.RelativeToParent((parent) => { return 0; }),
                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToParent((parent) => { return parent.Height; }));

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

            profilePicture.Source = ImageSource.FromUri(new Uri(UserView.ProfilePicUrl));

            return profilePicture;
        }

        //Gets the name of the current user.
        protected Label generateUserNameLabel()
        {
            Label nameLabel = new Label();

            nameLabel.TextColor = Color.Black;
            nameLabel.LineBreakMode = LineBreakMode.WordWrap;
            nameLabel.Text = UserView.Username.Substring(0, Math.Min(UserView.Username.IndexOf(" "), UserView.Username.Length));
            nameLabel.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            nameLabel.FontSize = 16;

            return nameLabel;
        }
    }
}
