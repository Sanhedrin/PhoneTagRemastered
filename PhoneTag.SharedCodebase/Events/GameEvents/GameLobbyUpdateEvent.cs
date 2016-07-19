using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class GameLobbyUpdateEvent : Event
    {
        public GameLobbyUpdateEvent(string i_GameId) 
        {
            GameId = i_GameId;
        }

        public string GameId { get; set; }
    }
}
