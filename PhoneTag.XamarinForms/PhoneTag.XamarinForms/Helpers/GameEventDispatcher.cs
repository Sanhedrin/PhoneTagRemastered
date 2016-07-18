using PhoneTag.SharedCodebase.Events;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Helpers
{
    /// <summary>
    /// A parser for the event push messages arriving from the server.
    /// </summary>
    public static class GameEventDispatcher
    {
        /// <summary>
        /// Takes the event acquired from the server, parses it and acts accordingly.
        /// </summary>
        public static void Parse(Event i_Event)
        {
            Device.BeginInvokeOnMainThread(() => TrailableContentPage.CurrentPage.ParseEvent(i_Event));
        }
    }
}
