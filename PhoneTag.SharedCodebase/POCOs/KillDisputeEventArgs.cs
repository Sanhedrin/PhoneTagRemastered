using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.POCOs
{
    public class KillDisputeEventArgs : EventArgs
    {
        public String DisputeId { get; set; }
        public String RoomId { get; set; }
        public String AttackerName { get; set; }
        public String AttackedName { get; set; }
        public String KillCamId { get; set; }
        public string AttackedId { get; set; }
        public string AttackerId { get; set; }

        public KillDisputeEventArgs(String i_DisputeId, String i_RoomId, String i_AttackedFBID, String i_AttackerFBID, String i_AttackedName, String i_AttackerName, String i_KillCamId)
        {
            DisputeId = i_DisputeId;
            RoomId = i_RoomId;
            AttackerName = i_AttackerName;
            AttackedName = i_AttackedName;
            AttackedId = i_AttackedFBID;
            AttackerId = i_AttackerFBID;
            KillCamId = i_KillCamId;
        }
    }
}

