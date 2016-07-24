using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PhoneTag.SharedCodebase.POCOs;
using PhoneTag.WebServices.Models;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.WebServices.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.SharedCodebase.Views;

namespace PhoneTag.WebServices.Controllers
{
    /// <summary>
    /// A controller that handles dispute requests.
    /// </summary>
    public class DisputeController : ApiController
    {
        /// <summary>
        /// Gets the dispute by the given id and returns a view of it.
        /// </summary>
        [Route("api/disputes/{i_DisputeId}")]
        [HttpGet]
        public async Task<DisputeView> GetDispute([FromUri] string i_DisputeId)
        {
            Dispute foundDispute = await GetDisputeModel(i_DisputeId);

            return (foundDispute != null) ? await foundDispute.GenerateView() : null;
        }

        /// <summary>
        /// Votes about the given dispute.
        /// </summary>
        [Route("api/disputes/{i_DisputeId}/vote")]
        [HttpPost]
        public async Task Vote([FromUri] string i_DisputeId, [FromBody] bool i_Vote)
        {
            if (!String.IsNullOrEmpty(i_DisputeId))
            {
                Dispute dispute = await GetDisputeModel(i_DisputeId);

                if(dispute != null)
                {
                    dispute.Vote(i_Vote);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid dispute ID given");
            }
        }

        public static async Task<Dispute> CreateDispute(KillDisputeEventArgs i_DisputeDetails)
        {
            Dispute dispute = null;

            if (i_DisputeDetails != null)
            {
                try
                {
                    dispute = new Dispute(i_DisputeDetails);
                    await Mongo.Database.GetCollection<Dispute>("Disputes").InsertOneAsync(dispute);

                    //Add the room to the expiration list.
                    ExpirationEntry expiration = new ExpirationEntry();
                    expiration.ExpirationTime = DateTime.Now.AddSeconds(30);
                    expiration._id = dispute._id;
                    await Mongo.Database.GetCollection<ExpirationEntry>("DisputeExpiration").InsertOneAsync(expiration);
                }
                catch (Exception e)
                {
                    dispute = null;
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }

            return dispute;
        }

        public static async Task<Dispute> GetDisputeModel(string i_DisputeId)
        {
            Dispute foundDispute = null;

            if (!String.IsNullOrEmpty(i_DisputeId))
            {
                try
                {
                    FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(i_DisputeId));

                    IMongoCollection<BsonDocument> disputes = Mongo.Database.GetCollection<BsonDocument>("Disputes");

                    if (await disputes.CountAsync(filter) > 0)
                    {
                        using (IAsyncCursor<Dispute> cursor = await disputes.FindAsync<Dispute>(filter))
                        {
                            foundDispute = await cursor.SingleAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    foundDispute = null;
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }
            else
            {
                ErrorLogger.Log("Invalid ID given");
            }

            return foundDispute;
        }
    }
}
