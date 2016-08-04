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

        public KillConfirmationDialog(KillRequestEvent i_KillRequest, String i_DisplayMessage)
        {
            initializeComponent(null, i_KillRequest, i_DisplayMessage);
        }

        public KillConfirmationDialog(String i_Dispute, KillRequestEvent i_KillRequest, String i_DisplayMessage)
        {
            initializeComponent(i_Dispute, i_KillRequest, i_DisplayMessage);
        }

        private void DenyButton_Clicked(object sender, EventArgs e)
        {
            if (KillDenied != null)
            {
                KillDenied(this, m_KillRequest);
            }
        }

        private void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            if (KillConfirmed != null)
            {
                KillConfirmed(this, m_KillRequest);
            }
        }
    }
}
