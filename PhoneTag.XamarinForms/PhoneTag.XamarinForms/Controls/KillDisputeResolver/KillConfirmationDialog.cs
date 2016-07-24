using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.POCOs;
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
        public event EventHandler<KillDisputeEventArgs> KillConfirmed;
        public event EventHandler<KillDisputeEventArgs> KillDenied;

        public KillConfirmationDialog(KillRequestEvent i_KillRequest)
        {
            initializeComponent(null, i_KillRequest);
        }

        public KillConfirmationDialog(String i_Dispute, KillRequestEvent i_KillRequest)
        {
            initializeComponent(i_Dispute, i_KillRequest);
        }
    }
}
