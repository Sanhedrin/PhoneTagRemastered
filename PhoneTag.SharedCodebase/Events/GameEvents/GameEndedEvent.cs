using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class GameEndedEvent : Event
    {
        public List<String> WinnerIds { get; set; }

        public GameEndedEvent(List<String> i_WinnerIds)
        {
            WinnerIds = i_WinnerIds;
        }
    }
}