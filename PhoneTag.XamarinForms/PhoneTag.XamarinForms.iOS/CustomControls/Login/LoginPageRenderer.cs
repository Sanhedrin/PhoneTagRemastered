using Facebook;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.Login;
using PhoneTag.XamarinForms.iOS.CustomControls.Login;
using PhoneTag.XamarinForms.Pages;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace PhoneTag.XamarinForms.iOS.CustomControls.Login
{
    public class LoginPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            login();
        }

        //Logs in to facebook
        private async void login()
        {
            Account userAccount = await FBLoginService.GetCurrentAccount();

            if (userAccount == null)
            {
                fullLogin();
            }
            else
            {
                authenticateUser(userAccount);
            }
        }

        //Displays the login dialog
        private void fullLogin()
        {
            OAuth2Authenticator auth = new OAuth2Authenticator(
                clientId: Keys.FacebookAppId,
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.Completed += Login_OnCompleted;
            auth.Error += Login_Error;

            PresentViewController(auth.GetUI(), true, null);
        }

        //Called when login fails.
        private void Login_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            LoadingPage.LoginFailedAction();
        }

        //Called when login succeeds
        private void Login_OnCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                authenticateUser(e.Account);
            }
            else
            {
                LoadingPage.LoginFailedAction();
            }
        }

        //Tries to connect the user using the given access token
        private async void authenticateUser(Account i_UserAccount)
        {
            FacebookClient client = null;

            try
            {
                String accessToken = i_UserAccount.Properties["access_token"];
                client = new FacebookClient(accessToken);
            }
            catch (Exception e)
            {
                //If we couldn't authenticate, we'll try to login properly.
                fullLogin();
            }

            if (client == null)
            {
                UserSocialView socialInfo = new UserSocialView();

                //Get basic user info.
                IDictionary<String, object> info = (IDictionary<String, object>)await client.GetTaskAsync("me?fields=id,name,email,picture");

                socialInfo.Id = (string)info["id"];
                socialInfo.Name = (string)info["name"];

                //Gets user profile picture url
                IDictionary<String, object> picture = (IDictionary<String, object>)info["picture"];
                IDictionary<String, object> pictureData = (IDictionary<String, object>)picture["data"];
                socialInfo.ProfilePictureUrl = (string)pictureData["url"];

                await LoadingPage.SuccessfulLoginAction(socialInfo, i_UserAccount);
            }
            else
            {
                LoadingPage.LoginFailedAction();
            }
        }
    }
}
