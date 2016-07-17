using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.WebServices.Events.OpLogEvents
{
    public class ExpirationEntry
    {
        public ObjectId _id { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}