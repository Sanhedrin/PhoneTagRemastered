using PhoneTag.WebServices.Events;
using PhoneTag.WebServices.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Helpers
{
    /// <summary>
    /// A parser for the event push messages arriving from the server.
    /// </summary>
    public static class GameEventDispatcher
    {
        /// <summary>
        /// Takes the event acquired from the server, parses it and acts accordingly.
        /// </summary>
        public static void Parse(Event i_Event)
        {
            if(i_Event is GameStartEvent)
            {
                parseGameStartedEvent(i_Event as GameStartEvent);
            }
            else if(i_Event is MessageEvent)
            {
                parseMessageEvent(i_Event as MessageEvent);
            }
        }

        //Parses the game started event to start the game.
        private static void parseGameStartedEvent(GameStartEvent gameStartEvent)
        {
        }

        //Parses the message event to display a new message to this user.
        private static void parseMessageEvent(MessageEvent messageEvent)
        {
        }
    }
}
