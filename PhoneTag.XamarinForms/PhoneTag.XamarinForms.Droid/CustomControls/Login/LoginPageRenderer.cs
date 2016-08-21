using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using PhoneTag.XamarinForms.Droid.CustomControls.Login;
using PhoneTag.XamarinForms.Pages;
using Xamarin.Forms;
using Android.Graphics;
using Xamarin.Auth;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.XamarinForms.Controls.Login;
using Facebook;
using PhoneTag.SharedCodebase.Views;
using Plugin.Settings;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.Login
{
    public class LoginPageRenderer : PageRenderer
    {
        private bool m_TriedFullLogin = false;

        public LoginPageRenderer() : base()
        {
            login();
        }

        //Logs in to facebook
        private async Task login()
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
            m_TriedFullLogin = true;

            OAuth2Authenticator auth = new OAuth2Authenticator(
                clientId: Keys.FacebookAppId,
                scope: "user_friends",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.Completed += Login_OnCompleted;
            auth.Error += Login_Error;

            Activity activity = this.Context as Activity;
            activity.StartActivity(auth.GetUI(activity));
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
        private async Task authenticateUser(Account i_UserAccount)
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

            if (client != null)
            {
                try
                {
                    UserSocialView socialInfo = new UserSocialView();

                    //Get basic user info.
                    IDictionary<String, object> info = (IDictionary<String, object>)await client.GetTaskAsync("me?fields=id,name,email,picture,friends");

                    socialInfo.Id = (string)info["id"];
                    socialInfo.Name = (string)info["name"];

                    //Gets user profile picture url
                    IDictionary<String, object> picture = (IDictionary<String, object>)info["picture"];
                    IDictionary<String, object> pictureData = (IDictionary<String, object>)picture["data"];
                    socialInfo.ProfilePictureUrl = (string)pictureData["url"];

                    //Gets friend list
                    IDictionary<String, object> friends = (IDictionary<String, object>)info["friends"];
                    IList<object> friendList = (IList<object>)friends["data"];
                    foreach(object friend in friendList)
                    {
                        socialInfo.FriendList.Add((string)((IDictionary<String, object>)friend)["id"]);
                    }

                    //If the user doesn't already exist in the database, this will add them.
                    await UserView.TryGetUser(socialInfo);

                    await LoadingPage.SuccessfulLoginAction(socialInfo, i_UserAccount);
                }
                catch (Exception e)
                {
                    if (!m_TriedFullLogin)
                    {
                        fullLogin();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        LoadingPage.LoginFailedAction();
                    }
                }
            }
            else
            {
                LoadingPage.LoginFailedAction();
            }
        }
    }
}
