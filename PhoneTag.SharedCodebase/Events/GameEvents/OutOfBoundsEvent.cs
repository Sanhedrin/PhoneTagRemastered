using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class OutOfBoundsEvent : Event
    {
        public String Message { get; set; }
        public String MessageTargetFBID { get; set; }

        public OutOfBoundsEvent(String i_Message, String i_TargetFBID)
        {
            Message = i_Message;
            MessageTargetFBID = i_TargetFBID;
        }
    }
}
