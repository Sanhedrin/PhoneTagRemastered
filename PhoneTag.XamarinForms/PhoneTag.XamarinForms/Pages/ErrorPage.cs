using PhoneTag.SharedCodebase.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// Displays an error that requires app restart.
    /// </summary>
    public partial class ErrorPage : TrailableContentPage
    {
        public ErrorPage(String i_ErrorMessage) : base()
        {
            initializeComponent(i_ErrorMessage);
        }

        private void RestartAppButton_Clicked()
        {
            Application.Current.MainPage = new LoadingPage();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            throw new NotImplementedException();
        }
    }
}
