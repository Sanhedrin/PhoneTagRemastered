using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Events.GameEvents;
using Xamarin.Forms;
using PhoneTag.XamarinForms.Controls.Login;
using PhoneTag.SharedCodebase.StaticInfo;
using Plugin.Settings;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class SettingsMenuPage : TrailableContentPage
    {
        public SettingsMenuPage()
        {
            initializeComponent();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
        }

        private async Task showTips()
        {
            DisplayAlert("Tips showing", "Tips will now show again for all menus.", "Ok");

            CrossSettings.Current.AddOrUpdateValue("AreaChooser", true);

            List<String> gameModeNames = await PhoneTagInfo.GetGameModeList();

            foreach(String modeName in gameModeNames)
            {
                CrossSettings.Current.AddOrUpdateValue(modeName, true);
            }
        }

        private async Task logout()
        {
            await FBLoginService.ClearAccounts();
            Application.Current.MainPage = new LoadingPage();
        }
    }
}
