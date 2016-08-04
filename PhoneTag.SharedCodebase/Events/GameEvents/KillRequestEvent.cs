using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.GameEvents
{
    public class KillRequestEvent : Event
    {
        public String RoomId { get; set; }
        public String RequestedBy { get; set; }
        public String KillCamId { get; set; }
        public string RequestedById { get; set; }
        public string AttackedPlayerId { get; set; }

        [JsonConstructor]
        public KillRequestEvent(String i_RoomId, String i_RequestedBy, String i_RequestedById, String i_KillCamId, String i_AttackedPlayerId)
        {
            RoomId = i_RoomId;
            RequestedBy = i_RequestedBy;
            KillCamId = i_KillCamId;
            RequestedById = i_RequestedById;
            AttackedPlayerId = i_AttackedPlayerId;
        }

        public KillRequestEvent(String i_RoomId, String i_RequestedBy, String i_RequestedById, String i_AttackedPlayerId)
        {
            RoomId = i_RoomId;
            RequestedBy = i_RequestedBy;
            KillCamId = null;
            RequestedById = i_RequestedById;
            AttackedPlayerId = i_AttackedPlayerId;
        }
    }
}
