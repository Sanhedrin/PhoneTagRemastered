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
    public partial class DisputeDialog : NotificationDialog
    {
        private KillDisputeEvent m_KillDisputeEvent;

        public DisputeDialog(KillDisputeEvent i_KillDisputeEvent) : base("Pending kill dispute")
        {
            m_KillDisputeEvent = i_KillDisputeEvent;
            
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            onOpened(new KillDisputeEventArgs(
                    m_KillDisputeEvent.DisputeDetails.DisputeId,
                    m_KillDisputeEvent.DisputeDetails.RoomId,
                    m_KillDisputeEvent.DisputeDetails.AttackedId,
                    m_KillDisputeEvent.DisputeDetails.AttackerId,
                    m_KillDisputeEvent.DisputeDetails.AttackedName,
                    m_KillDisputeEvent.DisputeDetails.AttackerName,
                    m_KillDisputeEvent.DisputeDetails.KillCamId));
        }
    }
}
