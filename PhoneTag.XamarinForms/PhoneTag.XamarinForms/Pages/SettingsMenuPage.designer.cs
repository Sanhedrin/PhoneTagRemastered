using PhoneTag.XamarinForms.Controls.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class SettingsMenuPage : TrailableContentPage
    {
        private void initializeComponent()
        {
            NavigationPage.SetHasBackButton(this, true);

            Button showTipsButton = generateShowTipsButton();
            Button logoutButton = generateLogoutButton();

            Title = "Settings";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                Children = {
                    showTipsButton,
                    logoutButton
                }
            };
        }

        private Button generateShowTipsButton()
        {
            Button logoutButton = new Button();

            logoutButton.Text = "Show tips";
            logoutButton.TextColor = Color.Black;
            logoutButton.BackgroundColor = Color.Gray;
            logoutButton.Command = new Command(() => { showTips(); });

            return logoutButton;
        }

        private Button generateLogoutButton()
        {
            Button logoutButton = new Button();

            logoutButton.Text = "Log out";
            logoutButton.TextColor = Color.Black;
            logoutButton.BackgroundColor = Color.Red;
            logoutButton.Command = new Command(() => { logout(); });

            return logoutButton;
        }
    }
}
