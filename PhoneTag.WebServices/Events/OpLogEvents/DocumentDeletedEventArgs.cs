using MongoDB.Bson;
using System;

namespace PhoneTag.WebServices.Events.OpLogEvents
{
    public class DocumentDeletedEventArgs : EventArgs
    {
        public ObjectId Id { get; set; }
        public String Collection { get; set; }

        public DocumentDeletedEventArgs() : base()
        {

        }

        public DocumentDeletedEventArgs(ObjectId i_Id, String i_Collection) : base()
        {
            Id = i_Id;
            Collection = i_Collection;
        }
    }
}