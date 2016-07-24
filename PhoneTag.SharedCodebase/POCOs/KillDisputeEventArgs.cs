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
        public String AttackerFBID { get; set; }
        public String AttackedFBID { get; set; }
        public String KillCamId { get; set; }

        public KillDisputeEventArgs(String i_DisputeId, String i_RoomId, String i_TargetFBID, String i_AttackerFBID, String i_KillCamId)
        {
            DisputeId = i_DisputeId;
            RoomId = i_RoomId;
            AttackerFBID = i_AttackerFBID;
            AttackedFBID = i_TargetFBID;
            KillCamId = i_KillCamId;
        }
    }
}

