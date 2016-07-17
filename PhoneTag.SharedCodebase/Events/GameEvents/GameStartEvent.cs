using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Events.GameEvents
{
    /// <summary>
    /// An event that occurs when the game starts from the lobby page.
    /// All players in the lobby should be sent into the game page.
    /// </summary>
    public class GameStartEvent : Event
    {
    }
}
