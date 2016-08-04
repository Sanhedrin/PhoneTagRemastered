using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class GameEventArrivedArgs : EventArgs
    {
        public Event Event { get; set; }

        public GameEventArrivedArgs(Event i_NewEvent)
        {
            Event = i_NewEvent;
        }
    }
}
