using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class ChatMessageEvent : Event
    {
        public String PlayerFBID { get; set; }
        public String PlayerName { get; set; }
        public String Message { get; set; }

        public ChatMessageEvent(String i_PlayerFBID, String i_PlayerName, String i_Message)
        {
            PlayerFBID = i_PlayerFBID;
            PlayerName = i_PlayerName;
            Message = i_Message;
        }
    }
}
