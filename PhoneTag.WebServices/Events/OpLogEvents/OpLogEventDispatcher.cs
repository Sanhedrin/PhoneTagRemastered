using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Events.OpLogEvents
{
    /// <summary>
    /// A dispatcher for oplog events in the database.
    /// </summary>
    public static class OpLogEventDispatcher
    {
        public static event EventHandler<DocumentDeletedEventArgs> DocumentDeleted;
        
        //Starts the listener.
        public static void Init()
        {
            listenForOpLogEvents();
        }

        //Listens to oplog events and dispatches the fitting event.
        private static async Task listenForOpLogEvents()
        {
            IMongoCollection<BsonDocument> oplog = Mongo.LocalDatabase.GetCollection<BsonDocument>("oplog.rs");
            BsonTimestamp lastTimestamp = oplog.Find(FilterDefinition<BsonDocument>.Empty).Sort(Builders<BsonDocument>.Sort.Descending("$natural")).First().GetValue("ts").AsBsonTimestamp;
            
            for (;;)
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Gt("ts", lastTimestamp);

                using (IAsyncCursor<BsonDocument> cursor = await oplog.FindAsync(filter, new FindOptions<BsonDocument> {
                    CursorType = CursorType.TailableAwait,
                    Sort = Builders<BsonDocument>.Sort.Ascending("$natural") }))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<BsonDocument> batch = cursor.Current;

                        foreach (BsonDocument entry in batch)
                        {
                            processOpLogEntry(entry);
                        }
                    }
                }

            }
        }

        //Processes the found oplog entry and dispatches the fitting event.
        private static async Task processOpLogEntry(BsonDocument entry)
        {
            if (entry["op"].AsString.Equals("d"))
            {
                if(DocumentDeleted != null)
                {
                    DocumentDeleted(null, new DocumentDeletedEventArgs(
                        entry["o"].AsBsonDocument["_id"].AsObjectId, 
                        entry["ns"].AsString.Substring(entry["ns"].AsString.IndexOf(".") + 1)));
                }
            }
        }
    }
}
