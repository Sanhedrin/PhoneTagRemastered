using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.XamarinForms.Controls.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class LoginPage : TrailableContentPage
    {
        public LoginPage() : base()
        {
            initializeComponent();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
        }
    }
}
