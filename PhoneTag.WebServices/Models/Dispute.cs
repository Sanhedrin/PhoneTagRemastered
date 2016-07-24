using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.SharedCodebase.POCOs;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.WebServices.Controllers;
using PhoneTag.WebServices.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.WebServices.Models
{
    public class Dispute : IViewable
    {
        public ObjectId _id { get; set; }
        public String RoomId { get; set; }
        public String AttackerId { get; set; }
        public String AttackedId { get; set; }
        public String KillCamId { get; set; }
        public Dictionary<bool, int> Votes { get; set; }

        public Dispute(KillDisputeEventArgs i_DisputeDetails)
        {
            AttackerId = i_DisputeDetails.AttackerFBID;
            AttackedId = i_DisputeDetails.AttackedFBID;
            KillCamId = i_DisputeDetails.KillCamId;

            //Initializes our voting option counters.
            Votes = new Dictionary<bool, int>() { { true, 0 }, { false, 0 } };
        }

        //Casts a vote.
        public async Task Vote(bool i_Vote)
        {
            Votes[i_Vote]++;

            try
            {
                //Update the room to add the player to it.
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set("Votes", Votes);

                await Mongo.Database.GetCollection<BsonDocument>("Disputes").UpdateOneAsync(filter, update);
            }
            catch(Exception e)
            {
                ErrorLogger.Log($"{e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }

        /// <summary>
        /// Expires the dispute, resolving it.
        /// </summary>
        public async Task Expire()
        {
            if (Votes != null && Votes.ContainsKey(true) && Votes.ContainsKey(false))
            {
                GameRoom room = await RoomController.GetRoomModel(RoomId);

                if (room != null)
                {
                    //If most votes determined that the player wasn't killed.
                    if (Votes[true] < Votes[false])
                    {
                        //We kill the attacking player for failing an assault.
                        room.KillPlayer(AttackerId);
                    }
                    else
                    {
                        //Otherwise, we kill the assasination target.
                        room.KillPlayer(AttackedId);
                    }

                    //And finally, remove the concluded dispute from the database.
                    FilterDefinition<BsonDocument> roomFilter = Builders<BsonDocument>.Filter.Eq("_id", _id);
                    await Mongo.Database.GetCollection<BsonDocument>("Disputes").DeleteOneAsync(roomFilter);
                }
            }
            else
            {
                ErrorLogger.Log("Invalid vote details found in database.");
            }
        }

        public async Task<dynamic> GenerateView()
        {
            DisputeView disputeView = new DisputeView();

            disputeView.DisputeId = _id.ToString();
            disputeView.RoomId = RoomId;
            disputeView.AttackerId = AttackerId;
            disputeView.AttackedId = AttackedId;
            disputeView.KillCamId = KillCamId;

            return disputeView;
        }
    }
}