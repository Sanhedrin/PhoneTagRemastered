using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    /// <summary>
    /// An event that occurs when the game starts from the lobby page.
    /// All players in the lobby should be sent into the game page.
    /// </summary>
    public class GameStartEvent : Event
    {
        public string GameId { get; set; }

        public GameStartEvent(string i_RoomId)
        {
            GameId = i_RoomId;
        }
    }
}
