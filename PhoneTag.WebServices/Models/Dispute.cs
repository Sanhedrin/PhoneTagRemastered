using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
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
        private readonly string v_Spare = false.ToString();
        private readonly string v_Kill = true.ToString();

        public ObjectId _id { get; set; }
        public String RoomId { get; set; }
        public String AttackerId { get; set; }
        public String AttackedId { get; set; }
        public String KillCamId { get; set; }
        public Dictionary<String, int> Votes { get; set; }

        public Dispute(KillDisputeEventArgs i_DisputeDetails)
        {
            RoomId = i_DisputeDetails.RoomId;
            AttackerId = i_DisputeDetails.AttackerFBID;
            AttackedId = i_DisputeDetails.AttackedFBID;
            KillCamId = i_DisputeDetails.KillCamId;

            //Initializes our voting option counters.
            Votes = new Dictionary<String, int>() { { v_Kill, 0 }, { v_Spare, 0 } };
        }

        //Casts a vote.
        public async Task Vote(bool i_Vote)
        {
            Votes[i_Vote.ToString()]++;

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
            if (Votes != null && Votes.ContainsKey(v_Kill) && Votes.ContainsKey(v_Spare))
            {
                GameRoom room = await RoomController.GetRoomModel(RoomId);

                if (room != null)
                {
                    //If most votes determined that the player wasn't killed.
                    if (Votes[v_Kill] < Votes[v_Spare])
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