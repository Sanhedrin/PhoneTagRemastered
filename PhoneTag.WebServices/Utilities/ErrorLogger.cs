using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Utilities
{
    public static class ErrorLogger
    {
        private const long k_MaxErrorMessages = 20;

        /// <summary>
        /// Logs an error message to the error log collection, will not log more than 50 at a time.
        /// </summary>
        /// <param name="i_Message"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Log(String i_Message, [CallerFilePath] String i_CallFilePath = "", [CallerLineNumber] int i_CallerLineNumber = 0)
        {
            long currentErrorCount = Mongo.Database.GetCollection<BsonDocument>("ErrorLog").Count(Builders<BsonDocument>.Filter.Empty);

            if (currentErrorCount < k_MaxErrorMessages - 1)
            {
                Mongo.Database.GetCollection<BsonDocument>("ErrorLog").InsertOne(new BsonDocument() {
                    { "Message", String.Format("~{0}:{1}~: {2}",
                        i_CallFilePath, i_CallerLineNumber,
                        i_Message.Substring(0, Math.Min(i_Message.Length, 1024))) } });
            }
            else if (currentErrorCount == k_MaxErrorMessages - 1)
            {
                Mongo.Database.GetCollection<BsonDocument>("ErrorLog").InsertOne(new BsonDocument() { { "Message", "Over 50 errors found, stopping" } });
            }
        }
    }
}
