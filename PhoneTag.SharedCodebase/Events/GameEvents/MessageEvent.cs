using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Events.GameEvents
{
    /// <summary>
    /// An event that occurs when a player receives a message.
    /// </summary>
    public class MessageEvent : Event
    {
        public String From { get; set; }
        public String Message { get; set; }
    }
}
