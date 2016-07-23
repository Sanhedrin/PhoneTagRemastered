using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
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
            Navigation.PushAsync(new CreateGamePage());
        }

        private void FindGameButton_Clicked()
        {
            Navigation.PushAsync(new GameSearchPage());
        }

        public override void ParseEvent(Event i_EventDetails)
        {
        }
    }
}
