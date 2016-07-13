﻿using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.StaticInfo;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
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
        private async Task initGame()
        {
            if (await PhoneTagInfo.ValidateVersion())
            {
                promptUserLogin();
            }
            else
            {
                Application.Current.MainPage = new ErrorPage(String.Format("Version mismatch. {0}Please update your game to the latest version.", Environment.NewLine));
            }
        }

        //To use this app, one must be logged in via their Facebook account.
        private void promptUserLogin()
        {
            Application.Current.MainPage = new LoginPage();
        }

        /// <summary>
        /// Called once login is successful
        /// </summary>
        /// <param name="i_AccessToken">The generated access token generated by the login.</param>
        public static async Task SuccessfulLoginAction(UserSocialView i_SocialView, Account i_UserAccount)
        {
            await FBLoginService.StoreAccount(i_SocialView, i_UserAccount);
            UserView.SetLoggedInUser(await UserView.TryGetUser(i_SocialView));
            proceedToMainMenuPage();
        }

        //Once authenticated, we can move on to the main menu page.
        private static void proceedToMainMenuPage()
        {
            // The root page of your application
            Application.Current.MainPage = new NavigationPage(new MainMenuPage());
        }

        /// <summary>
        /// Called once the login fails.
        /// </summary>
        public static void LoginFailedAction()
        {
            Application.Current.MainPage = new ErrorPage(String.Format("Login failed. {0}Please try logging in again.", Environment.NewLine));
        }
    }
}
