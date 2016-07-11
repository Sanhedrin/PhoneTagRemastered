using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhoneTag.SharedCodebase.Views;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// The server info model.
    /// </summary>
    public class ServerInfo : IViewable
    {
        public ObjectId _id { get; set; }
        public String Version { get; set; }

        /// <summary>
        /// Generates a view for this model.
        /// </summary>
        public async Task<dynamic> GenerateView()
        {
            ServerInfoView serverInfoView = new ServerInfoView();

            serverInfoView.Version = this.Version;

            return serverInfoView;
        }
    }
}