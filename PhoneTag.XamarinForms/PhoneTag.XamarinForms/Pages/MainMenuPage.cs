using PhoneTag.WebServices.Views;
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
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
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
    }
}
