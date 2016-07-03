using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xamarin.Forms;

using Plugin.Media.Abstractions;

using PhoneTag.XamarinForms.Pages;

namespace PhoneTag.XamarinForms
{
    public class App : Application
    {        
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new GamePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
