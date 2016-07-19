using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.SharedCodebase.Models;
using PhoneTag.WebServices.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.SharedCodebase.Controllers.ExpirationControllers
{
    public class UserExpirationController
    {
        /// <summary>
        /// Initializes the UsersController's timeout event listeners.
        /// </summary>
        public static void InitUserExpirationController()
        {
            //Sets up a listener for user time outs.
            OpLogEventDispatcher.DocumentDeleted += OpLogEventDispatcher_DocumentDeleted;
        }

        //Listens for user timeouts.
        private static void OpLogEventDispatcher_DocumentDeleted(object sender, DocumentDeletedEventArgs e)
        {
            if (e.Collection.Equals("UserExpiration"))
            {
                handleUserExpiration(e.Id);
            }
        }

        //When a user expires that means that they've gone inactive.
        //We'll mark them as inactive and remove them from the current room if they're in one.
        private static async Task handleUserExpiration(ObjectId i_ExpiredId)
        {
            if (i_ExpiredId != null)
            {
                User user = await UsersController.GetUserModel(i_ExpiredId.ToString());

                await UsersController.Quit(i_ExpiredId);
            }
        }
    }
}