using MongoDB.Bson;
using MongoDB.Driver;
using PhoneTag.WebServices;
using PhoneTag.WebServices.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Events.OpLogEvents
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
            try
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
            catch (Exception e)
            {
                ErrorLogger.Log(e.Message);
            }
        }

        //Processes the found oplog entry and dispatches the fitting event.
        private static void processOpLogEntry(BsonDocument entry)
        {
            //If the oplog entry doesn't have an operation value or a namespace value then it's of no interest
            if (entry != null && entry.Contains("op") && entry["op"].IsString
                && entry.Contains("ns") && entry["ns"].IsString)
            {
                String collectionName = entry["ns"].AsString.Contains(".") ?
                    entry["ns"].AsString.Substring(entry["ns"].AsString.IndexOf(".") + 1) :
                    entry["ns"].AsString;

                //We only care about deletion operations that aren't on the error log.
                if (entry["op"].AsString.Equals("d") && !collectionName.Equals("ErrorLog"))
                {
                    //If the deletion operation doesn't have an object it operated on, or that doesn't have na id
                    //then something is wrong with the format of the entry.
                    if (entry.Contains("o") && entry["o"].IsBsonDocument
                        && entry["o"].AsBsonDocument.Contains("_id") && entry["o"].AsBsonDocument["_id"].IsObjectId)
                    {
                        if (DocumentDeleted != null)
                        {

                            DocumentDeleted(null, new DocumentDeletedEventArgs(
                                entry["o"].AsBsonDocument["_id"].AsObjectId, collectionName));
                        }
                    }
                    else
                    {
                        ErrorLogger.Log(String.Format("Misformatted OpLog entry parsed: {0}", entry.ToString()));
                    }
                }
            }
        }
    }
}
