using PushNotification.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushNotification.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using com.shephertz.app42.paas.sdk.csharp;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using DeviceType = PushNotification.Plugin.Abstractions.DeviceType;
using App42DeviceType = com.shephertz.app42.paas.sdk.csharp.pushNotification.DeviceType;


namespace PhoneTag.XamarinForms.PushNotifications
{
    //Class to handle push notifications listens to events such as registration, unregistration, message arrival and errors.
    public class  CrossPushNotificationListener : IPushNotificationListener
    {
        private const int k_TokenAlreadyRegistered = 1700;

        public static PushNotificationService PushService { get; private set; }
		public static String DeviceToken { get; private set; }

        public void OnMessage(JObject i_Values, DeviceType i_DeviceType)
        {
            Debug.WriteLine("Message Arrived");
        }

        public void OnRegistered(string i_Token, DeviceType i_DeviceType)
        {
            DeviceToken = i_Token;
            initApp42PushNotifications(i_Token, i_DeviceType);
            Debug.WriteLine(string.Format("Push Notification - Device Registered - Token : {0}", i_Token));
        }

        public void OnUnregistered(DeviceType i_DeviceType)
        {
            Debug.WriteLine("Push Notification - Device Unnregistered");
       
        }

        public void OnError(string i_Message, DeviceType i_DeviceType)
        {
            Debug.WriteLine(string.Format("Push notification error - {0}", i_Message));
        }

        public bool ShouldShowNotification()
        {
            return true;
        }


        private void initApp42PushNotifications(string i_Token, DeviceType i_DeviceType)
        {
            App42API.Initialize("b7cce3f56c238389790ccef2a13c69fe88cb9447523730b6e93c849a6d0bd510",
                "e6672070bad36d0940805bff5d81fa3d9d66e440913301f1f438ad937b5d8502");
            App42API.SetLoggedInUser(i_Token);
			
            PushService = App42API.BuildPushNotificationService();

            String deviceType = i_DeviceType == DeviceType.Android ? App42DeviceType.ANDROID :
                (i_DeviceType == DeviceType.iOS ? App42DeviceType.iOS : null);

            try
            {
                PushService.StoreDeviceToken("DudeTest1", i_Token, deviceType);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains(k_TokenAlreadyRegistered.ToString()))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: on setting up push notifications: " + e.Message);
                }
            }
        }
    }
}
