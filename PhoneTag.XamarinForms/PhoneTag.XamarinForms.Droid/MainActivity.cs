﻿using System;

using Java.IO;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Net;

using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using PhoneTag.SharedCodebase.Utils;

namespace PhoneTag.XamarinForms.Droid
{
    [Activity(Label = "PhoneTag", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            if(App.Current.MainPage?.Navigation != null 
                && App.Current.MainPage.Navigation.NavigationStack.Count > 1)
            {
                base.OnBackPressed();
            }
        }
    }
}

