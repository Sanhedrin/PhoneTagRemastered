using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class KillRequestEvent : Event
    {
        public String RequestedBy { get; set; }
        public String KillCamId { get; set; }

        public KillRequestEvent(String i_RequestedBy, String i_KillCamId)
        {
            RequestedBy = i_RequestedBy;
            KillCamId = i_KillCamId;
        }
    }
}
