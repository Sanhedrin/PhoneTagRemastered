using MongoDB.Bson;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.WebServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.WebServices.Controllers.ExpirationControllers
{
    public class DisputeExpirationController
    {
        /// <summary>
        /// Initializes the RoomController timeout event listeners.
        /// </summary>
        public static void InitDisputeExpirationController()
        {
            OpLogEventDispatcher.DocumentDeleted += OpLogEventDispatcher_DocumentDeleted;
        }

        //Listens for room timeouts.
        private static void OpLogEventDispatcher_DocumentDeleted(object sender, DocumentDeletedEventArgs e)
        {
            if (e.Collection.Equals("RoomExpiration"))
            {
                handleRoomExpiration(e.Id);
            }
        }

        //Cancels the waiting room.
        //A room that already started playing doesn't expire.
        private static async Task handleRoomExpiration(ObjectId i_ExpiredId)
        {
            Dispute dispute = await DisputeController.GetDisputeModel(i_ExpiredId.ToString());

            if (dispute != null)
            {
                dispute.Expire();
            }
        }
    }
}