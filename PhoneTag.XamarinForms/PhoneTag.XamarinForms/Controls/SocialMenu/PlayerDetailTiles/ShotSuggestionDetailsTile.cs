using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Pages;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    public class ShotSuggestionDetailsTile : PlayerDetailsTile
    {
        public ShotSuggestionDetailsTile(UserView i_UserView) : base(i_UserView)
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

        private StackLayout generateDetailStack()
        {
            StackLayout layout = new StackLayout();
            
            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Start };
            layout.VerticalOptions = new LayoutOptions() { Alignment = LayoutAlignment.Center };
            
            layout.Children.Add(profilePic);
            layout.Children.Add(nameLabel);

            return layout;
        }

        private StackLayout generateActionStack()
        {
            StackLayout layout = new StackLayout();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            Button chooseTargetButton = generateChooseTargetButton();

            layout.Children.Add(chooseTargetButton);

            return layout;
        }

        private Button generateChooseTargetButton()
        {
            Button chooseTargetButton = new Button();

            chooseTargetButton.Text = "Choose target";
            chooseTargetButton.Command = new Command(() => 
            {
                UserView.Current?.TryKill(m_UserView.FBID, ShotDisplayPage.LastKillCam);
                Application.Current.MainPage.Navigation.PopAsync();
            });

            return chooseTargetButton;
        }
    }
}
