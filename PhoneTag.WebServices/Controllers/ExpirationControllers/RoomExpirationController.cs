using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.SharedCodebase.Models;
using PhoneTag.WebServices;
using PhoneTag.WebServices.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.SharedCodebase.Controllers.ExpirationControllers
{
    public class RoomExpirationController
    {
        /// <summary>
        /// Initializes the RoomController timeout event listeners.
        /// </summary>
        public static void InitRoomExpirationController()
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
            if(i_ExpiredId != null)
            { 
                GameRoom room = await RoomController.GetRoomModel(i_ExpiredId.ToString());

                if (room != null)
                {
                    if (!room.Started)
                    {
                        foreach (String userId in room.LivingUsers)
                        {
                            await UsersController.LeaveRoom(userId);
                        }

                        try
                        {
                            FilterDefinition<BsonDocument> roomFilter = Builders<BsonDocument>.Filter.Eq("_id", i_ExpiredId);
                            await Mongo.Database.GetCollection<BsonDocument>("Rooms").DeleteOneAsync(roomFilter);
                        }
                        catch (Exception e)
                        {
                            ErrorLogger.Log(e.Message);
                        }
                    }
                }
            }
        }
    }
}