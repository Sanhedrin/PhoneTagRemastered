using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using PhoneTag.SharedCodebase.Events.GameEvents;

namespace PhoneTag.SharedCodebase.Utils
{
    public static class PushNotificationUtils
    {
        public static void PushEvent(Event i_Event, List<string> i_SendTo)
        {
            String gameStartEventMessage = JsonConvert.SerializeObject(i_Event, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            gameStartEventMessage = gameStartEventMessage.Replace('\"', '\'');

            PushNotificationService pushService = App42API.BuildPushNotificationService();
            pushService.SendPushMessageToGroup(gameStartEventMessage, i_SendTo);
        }
    }
}
