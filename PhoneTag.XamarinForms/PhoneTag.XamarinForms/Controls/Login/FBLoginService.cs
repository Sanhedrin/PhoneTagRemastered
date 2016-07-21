using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Newtonsoft.Json;
using PhoneTag.SharedCodebase.Views;

namespace PhoneTag.XamarinForms.Controls.Login
{
    /// <summary>
    /// Class used to verify the active user is logged in.
    /// </summary>
    public static class FBLoginService
    {
        //Used to save performance when possible by not reading the values a second time in the same app run.
        private static Account s_LoginAccount = null;

        private static AccountStore s_AccountStore = null;
        private static AccountStore CurrentAccountStore {
            get
            {
                if(s_AccountStore == null)
                {
                    s_AccountStore = AccountStore.Create();
                }

                return s_AccountStore;
            }
        }

        /// <summary>
        /// Gets the currently used user account
        /// </summary>
        /// <returns></returns>
        public static async Task<Account> GetCurrentAccount()
        {
            List<Account> accounts = await CurrentAccountStore.FindAccountsForServiceAsync("Facebook");

            return accounts == null ? null : (accounts.Count > 0 ? accounts[0] : null);
        }

        /// <summary>
        /// Checks if the user is logged in.
        /// </summary>
        public static async Task<bool> IsLoggedIn()
        {
            if (!String.IsNullOrEmpty(LoggedInUserId))
            {
                UserView user = await UserView.GetUser(LoggedInUserId);

                if (user != null)
                {
                    UserView.SetLoggedInUser(user);

                    if (s_LoginAccount == null)
                    {
                        List<Account> storedAccounts = await CurrentAccountStore.FindAccountsForServiceAsync("Facebook");
                        s_LoginAccount = storedAccounts == null ? null : (storedAccounts.Count > 0 ? storedAccounts[0] : null);
                    }
                }
            }

            return s_LoginAccount != null;
        }

        /// <summary>
        /// Gets the id of the logged in user.
        /// </summary>
        public static String LoggedInUserId
        {
            get
            {
                UserSocialView socialView = JsonConvert.DeserializeObject<UserSocialView>(
                    CrossSettings.Current.GetValueOrDefault<String>("SocialInfo", null));

                return socialView?.Id;
            }
        }

        /// <summary>
        /// Saves a new user account.
        /// </summary>
        public static async Task StoreAccount(UserSocialView i_SocialView, Account i_UserAccount)
        {
            await ClearAccounts();
            await CurrentAccountStore.SaveAsync(i_UserAccount, "Facebook");

            try
            {
                CrossSettings.Current.AddOrUpdateValue<String>("SocialInfo", JsonConvert.SerializeObject(i_SocialView));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Deletes all stored accounts
        /// </summary>
        public static async Task ClearAccounts()
        {
            s_LoginAccount = null;

            List<Account> storedAccounts = await CurrentAccountStore.FindAccountsForServiceAsync("Facebook");

            if (storedAccounts != null)
            {
                for (int i = 0; i < storedAccounts.Count; ++i)
                {
                    await CurrentAccountStore.DeleteAsync(storedAccounts[i], "Facebook");
                }

                CrossSettings.Current.Remove("SocialInfo");
            }
        }
    }
}
