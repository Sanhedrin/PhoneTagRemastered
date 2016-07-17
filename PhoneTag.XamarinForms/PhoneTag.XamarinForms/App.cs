using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xamarin.Forms;

using Plugin.Media.Abstractions;

using PhoneTag.XamarinForms.Pages;
using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.WebServices.Utils;
using System.Threading.Tasks;
using PhoneTag.WebServices.StaticInfo;
using Plugin.Geolocator;
using System.Threading;
using System.Net.Http;
using PhoneTag.WebServices;
using PhoneTag.WebServices.Views;

namespace PhoneTag.XamarinForms
{
    public class App : Application
    {        
        public App()
        {
            CrossGeolocator.Current.DesiredAccuracy = 5;

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
            MainPage = new LoadingPage();
        }
    }
}
