using PhoneTag.SharedCodebase.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Pages
{
    interface IEventParsable
    {
        /// <summary>
        /// The page implementing this interface must know how to parse event that are relevant to it.
        /// </summary>
        /// <param name="i_EventDetails">Details of the event that has occured.</param>
        void ParseEvent(Event i_EventDetails);
    }
}
