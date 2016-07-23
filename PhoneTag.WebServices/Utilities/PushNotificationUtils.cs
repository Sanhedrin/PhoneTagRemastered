using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.WebServices;
using MongoDB.Bson;
using PhoneTag.WebServices.Utilities;

namespace PhoneTag.SharedCodebase.Utils
{
    public static class PushNotificationUtils
    {
        public static void PushEvent(Event i_Event, List<string> i_SendTo)
        {
            if (i_SendTo != null && i_SendTo.Count > 0)
            {
                try
                {
                    String eventMessage = JsonConvert.SerializeObject(i_Event, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                    eventMessage = eventMessage.Replace('\"', '\'');

                    PushNotificationService pushService = App42API.BuildPushNotificationService();
                    pushService.SendPushMessageToGroup(eventMessage, i_SendTo);
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }
        }
    }
}
