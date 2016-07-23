using PhoneTag.SharedCodebase.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.WebServices.Models.GameModes
{
    public class GameEndedEventArgs : EventArgs
    {
        public GameEndedEvent EventDetails { get; set; }

        public GameEndedEventArgs(GameEndedEvent i_GameEndedEvent)
        {
            EventDetails = i_GameEndedEvent;
        }
    }
}