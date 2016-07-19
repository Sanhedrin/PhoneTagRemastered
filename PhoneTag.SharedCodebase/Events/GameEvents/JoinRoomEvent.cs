using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class JoinRoomEvent : GameLobbyUpdateEvent
    {
        public JoinRoomEvent(string i_GameId) : base(i_GameId)
        {

        }
    }
}
