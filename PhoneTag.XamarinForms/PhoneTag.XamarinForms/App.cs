
using Xamarin.Forms;

using PhoneTag.XamarinForms.Pages;
using com.shephertz.app42.paas.sdk.csharp;
using Plugin.Geolocator;
using PhoneTag.SharedCodebase.Utils;

namespace PhoneTag.XamarinForms
{
    public class App : Application
    {        
        public App()
        {
            CrossGeolocator.Current.DesiredAccuracy = 0.05;
            
            MainPage = new LoadingPage();
            //MainPage = new NavigationPage(new GamePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            App42API.Initialize(Keys.App42APIKey, Keys.App42SecretKey);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
