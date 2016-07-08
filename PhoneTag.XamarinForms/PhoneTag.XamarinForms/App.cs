using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xamarin.Forms;

using Plugin.Media.Abstractions;

using PhoneTag.XamarinForms.Pages;
using PushNotification.Plugin;
using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.Utils;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.StaticInfo;

namespace PhoneTag.XamarinForms
{
    public class App : Application
    {        
        public App()
        {
            
            MainPage = new LoadingPage();
            //MainPage = new NavigationPage(new GamePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //CrossPushNotification.Current.Register();
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
