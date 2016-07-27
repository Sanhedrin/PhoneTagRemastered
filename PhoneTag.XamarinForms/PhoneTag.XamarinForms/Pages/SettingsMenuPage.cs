using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Events.GameEvents;
using Xamarin.Forms;
using PhoneTag.XamarinForms.Controls.Login;

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

        private async Task logout()
        {
            await FBLoginService.ClearAccounts();
            Application.Current.MainPage = new LoadingPage();
        }
    }
}
