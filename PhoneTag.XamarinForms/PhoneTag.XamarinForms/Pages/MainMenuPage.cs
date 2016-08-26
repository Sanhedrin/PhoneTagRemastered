using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
using Plugin.Geolocator;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The main menu.
    /// </summary>
    public partial class MainMenuPage : TrailableContentPage
    {
        public MainMenuPage() : base()
        {
            initializeComponent();
            UserView.Current.Login();
        }

        private void CreateGameButton_Clicked()
        {
            if (CrossGeolocator.Current.IsGeolocationEnabled && CrossGeolocator.Current.IsGeolocationAvailable)
            {
                Navigation.PushAsync(new CreateGamePage());
            }
            else
            {
                DisplayAlert("Can't create a game!", $"No GPS signal found.{Environment.NewLine}Please try enabling your GPS and then try again.", "Ok");
            }
        }

        private void FindGameButton_Clicked()
        {
            if (CrossGeolocator.Current.IsGeolocationEnabled && CrossGeolocator.Current.IsGeolocationAvailable)
            {
                Navigation.PushAsync(new GameSearchPage());
            }
            else
            {
                DisplayAlert("Can't search for a game!", $"No GPS signal found.{Environment.NewLine}Please try enabling your GPS and then try again.", "Ok");
            }
        }

        private void SettingsButton_Clicked()
        {
            Navigation.PushAsync(new SettingsMenuPage());
        }

        protected override bool OnBackButtonPressed()
        {
            m_FriendsButton.TriggerFriendMenu();

            return base.OnBackButtonPressed();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
        }
    }
}
