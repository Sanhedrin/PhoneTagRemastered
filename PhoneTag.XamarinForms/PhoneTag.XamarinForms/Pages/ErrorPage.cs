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
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage(String i_ErrorMessage)
        {
            initializeComponent(i_ErrorMessage);
        }

        private void ErrorPage_RestartAppButtonClicked()
        {
            Application.Current.MainPage = new LoadingPage();
        }
    }
}
