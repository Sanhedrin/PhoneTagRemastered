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
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using DeviceType = PushNotification.Plugin.Abstractions.DeviceType;
using App42DeviceType = com.shephertz.app42.paas.sdk.csharp.pushNotification.DeviceType;

namespace PhoneTag.XamarinForms.PushNotifications
{
    //Class to handle push notifications listens to events such as registration, unregistration, message arrival and errors.
    public class CrossPushNotificationListener : IPushNotificationListener
    {
        private PushNotificationService m_PushService;

        public void OnMessage(JObject i_Values, DeviceType i_DeviceType)
        {
            Debug.WriteLine("Message Arrived");
        }

        /// <summary>
        /// Occurs after the Register() method has been called, should only happen ONCE in the app's lifetime.
        /// </summary>
        /// <param name="i_Token">The ID generated for this user by the APNS, this value should be stored
        /// for future use.</param>
        public void OnRegistered(string i_Token, DeviceType i_DeviceType)
        {
            Debug.WriteLine(string.Format("Push Notification - Device Registered - Token : {0}", i_Token));

            String deviceType = i_DeviceType == DeviceType.Android ? App42DeviceType.ANDROID :
                (i_DeviceType == DeviceType.iOS ? App42DeviceType.iOS : null);

            m_PushService = App42API.BuildPushNotificationService();
            App42API.SetLoggedInUser("TestDude1");
            m_PushService.StoreDeviceToken("TestDude1", i_Token, deviceType);
        }

        public void OnUnregistered(DeviceType i_DeviceType)
        {
            Debug.WriteLine("Push Notification - Device Unnregistered");
       
        }

        public void OnError(string i_Message, DeviceType i_DeviceType)
        {
            Debug.WriteLine(string.Format("Push notification error - {0}",i_Message));
        }

        public bool ShouldShowNotification()
        {
            return true;
        }
    }
}
