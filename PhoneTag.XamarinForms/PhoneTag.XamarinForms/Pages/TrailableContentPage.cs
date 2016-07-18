using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Events.GameEvents;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// A content page that allows static access to information on the currently active page.
    /// </summary>
    public abstract class TrailableContentPage : ContentPage, IEventParsable
    {
        public static Type CurrentPageType { get; set; }
        public static TrailableContentPage CurrentPage { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CurrentPage = this;
            CurrentPageType = this.GetType();
        }

        public abstract void ParseEvent(Event i_EventDetails);
    }
}
