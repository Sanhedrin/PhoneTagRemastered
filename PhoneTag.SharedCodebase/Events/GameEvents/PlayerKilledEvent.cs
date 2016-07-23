using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class PlayerKilledEvent : Event
    {
        public String PlayerFBID { get; set; }

        public PlayerKilledEvent(String i_PlayerFBID)
        {
            PlayerFBID = i_PlayerFBID;
        }
    }
}
