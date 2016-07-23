using PhoneTag.SharedCodebase.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    /// <summary>
    /// A custom control that displays the kill confirmation dialog to a player.
    /// </summary>
    public partial class KillConfirmationDialog : StackLayout
    {
        public event EventHandler KillConfirmed, KillDenied;

        public KillConfirmationDialog(KillRequestEvent i_KillRequest)
        {
            initializeComponent(i_KillRequest);
        }
    }
}
