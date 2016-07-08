using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.StaticInfo;
using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The loading screen for the game.
    /// </summary>
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            initializeComponent();
            initGame();
        }

        //Verifies that the client's version matches that of the server.
        //If it does, we initialize the game and move to the main menu, otherwise the user is requested to
        //update their app.
        private async void initGame()
        {
            if (await PhoneTagInfo.ValidateVersion())
            {
                App42API.Initialize(Keys.App42APIKey, Keys.App42SecretKey);

                // The root page of your application
                Application.Current.MainPage = new NavigationPage(new MainMenuPage());
            }
            else
            {
                Application.Current.MainPage = new ErrorPage(String.Format("Version mismatch. {0}Please update your game to the latest version.", Environment.NewLine));
            }
        }
    }
}
