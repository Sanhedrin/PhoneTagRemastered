using PhoneTag.SharedCodebase.POCOs;
using PhoneTag.SharedCodebase.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class KillDisputeEvent : Event
    {
        public DisputeView DisputeDetails { get; set; }

        public KillDisputeEvent(DisputeView i_DisputeDetails)
        {
            DisputeDetails = i_DisputeDetails;
        }
    }
}
