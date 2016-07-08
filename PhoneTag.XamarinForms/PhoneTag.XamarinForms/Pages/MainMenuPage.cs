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
        }

        private void CreateGameButton_Clicked()
        {
            Navigation.PushAsync(new CreateGamePage());
        }

        private void FindGameButton_Clicked()
        {
            throw new NotImplementedException();
        }
    }
}
