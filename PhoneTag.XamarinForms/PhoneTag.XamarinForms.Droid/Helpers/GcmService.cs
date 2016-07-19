using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gcm.Client;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using Android.Util;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Views;
using System.Threading;
using PhoneTag.XamarinForms.Helpers;
using PhoneTag.SharedCodebase.Events;
using Newtonsoft.Json;
using PhoneTag.SharedCodebase.Events.GameEvents;

namespace PhoneTag.XamarinForms.Droid.Helpers
{
    //You must subclass this!
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        //IMPORTANT: Change this to your own Sender ID!
        //The SENDER_ID is your Google API Console App Project ID.
        //  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
        //  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
        //  where 785671162406 is the project id, which is the SENDER_ID to use!
        public static string[] SENDER_IDS = new string[] { "232827258785" };

        public const string TAG = "PushSharp-GCM";
    }

    [Service] //Must use the service tag
    public class PushHandlerService : GcmServiceBase
    {
        public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

        const string TAG = "GCM-SAMPLE";

        protected override void OnRegistered(Context i_Context, string i_RegistrationId)
        {
            Log.Verbose(TAG, "GCM Registered: " + i_RegistrationId);
            //Eg: Send back to the server
            //	var result = wc.UploadString("http://your.server.com/api/register/", "POST", 
            //		"{ 'registrationId' : '" + registrationId + "' }");

            StoreRegistrationToken(i_RegistrationId);
        }

        /// <summary>
        /// Stores the given token in the App42 backened to support pushing messages to them.
        /// </summary>
        public static async Task StoreRegistrationToken(string i_RegistrationId)
        {
            while(UserView.Current == null) { await Task.Delay(100); }
            
            PushNotificationService pushService = App42API.BuildPushNotificationService();
            pushService.StoreDeviceToken(UserView.Current.FBID, i_RegistrationId, DeviceType.ANDROID);
        }

        protected override void OnUnRegistered(Context i_Context, string i_RegistrationId)
        {
            Log.Verbose(TAG, "GCM Unregistered: " + i_RegistrationId);

            deleteRegistrationToken(i_RegistrationId);

            createNotification("GCM Unregistered...", "The device has been unregistered, Tap to View!");
        }

        private async Task deleteRegistrationToken(string i_RegistrationId)
        {
            while (UserView.Current == null) { await Task.Delay(100); }
            
            PushNotificationService pushService = App42API.BuildPushNotificationService();
            pushService.DeleteDeviceToken(UserView.Current.Username, i_RegistrationId);
        }

        protected override void OnMessage(Context i_Context, Intent i_Intent)
        {
            Log.Info(TAG, i_Intent.Extras.Get("message").ToString());
            //createNotification("This", i_Intent.Extras.Get("message").ToString());

            try
            {
                object deserializedObject = JsonConvert.DeserializeObject(i_Intent.Extras.Get("message").ToString(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

                if (deserializedObject != null && deserializedObject is Event)
                {
                    GameEventDispatcher.Parse(deserializedObject as Event);
                }
            }
            catch (Exception e)
            {
                OnError(i_Context, "Received push message that wasn't deserializable to an Event type");
            }
        }

        protected override bool OnRecoverableError(Context i_Context, string i_ErrorId)
        {
            Log.Warn(TAG, "Recoverable Error: " + i_ErrorId);

            return base.OnRecoverableError(i_Context, i_ErrorId);
        }

        protected override void OnError(Context i_Context, string i_ErrorId)
        {
            Log.Error(TAG, "GCM Error: " + i_ErrorId);
        }

        void createNotification(string i_Title, string i_Desc)
        {
            //Create notification
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            //Create an intent to show ui
            var uiIntent = new Intent(this, typeof(MainActivity));

            //Create the notification
            var notification = new Notification(Android.Resource.Drawable.SymActionEmail, i_Title);

            //Auto cancel will remove the notification once the user touches it
            notification.Flags = NotificationFlags.AutoCancel;

            //Set the notification info
            //we use the pending intent, passing our ui intent over which will get called
            //when the notification is tapped.
            notification.SetLatestEventInfo(this, i_Title, i_Desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

            //Show the notification
            notificationManager.Notify(1, notification);
        }
    }
}