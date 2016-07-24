using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    public class DisputeView
    {
        public string DisputeId { get; set; }
        public String RoomId { get; set; }
        public string AttackedId { get; set; }
        public string AttackerId { get; set; }
        public string KillCamId { get; set; }

        public DisputeView()
        {

        }

        /// <summary>
        /// Gets the dispute object for this dispute.
        /// </summary>
        public static async Task<DisputeView> GetDispute(String i_DisputeId)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetMethodAsync(String.Format("disputes/{0}", i_DisputeId));
            }
        }

        /// <summary>
        /// Votes about the given dispute.
        /// </summary>
        public async Task Vote(bool i_Vote)
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync<bool>(String.Format("disputes/{0}/vote", DisputeId), i_Vote);
            }
        }
    }
}
