using PhoneTag.WebServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PhoneTag.WebServices.Views;
using Newtonsoft.Json.Linq;
using com.shephertz.app42.paas.sdk.csharp;
using Keys = PhoneTag.WebServices.Utils.Keys;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using PhoneTag.WebServices.Events;
using PhoneTag.WebServices.Events.GameEvents;

namespace PhoneTag.WSTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            App42API.Initialize(Keys.App42APIKey, Keys.App42SecretKey);
            InitializeComponent();
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            createUser();
        }

        private void btnGetUser_Click(object sender, EventArgs e)
        {
            getUser();
        }

        private void btnClearUser_Click(object sender, EventArgs e)
        {
            clearUser();
        }

        private async Task clearUser()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.GetMethodAsync("test/clear");

                tbResult.Text = "Cleared";
            }
        }

        private async Task createUser()
        {
            UserSocialView user = new UserSocialView()
            {
                Id = "000000000000000",
                Name = "TestUser",
                ProfilePictureUrl = "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xpf1/v/t1.0-1/p50x50/12509626_1091087887602511_1046699018755865026_n.jpg?oh=eb7d2acea0fb1a866dce99acc7b00d5b&oe=57F49E68&__gda__=1479833351_a28298e3486449c760c485d5e44664fd"
            };
            bool success = await UserView.CreateUser(user);

            tbResult.Text = success.ToString();
        }

        private async Task getUser()
        { 
            UserView user = await UserView.GetUser("000000000000000");

            tbResult.Text = JsonConvert.SerializeObject(user);
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            testPush();
        }

        private void testPush()
        {
            PushNotificationService pushService = App42API.BuildPushNotificationService();

            String gameStartEventMessage = JsonConvert.SerializeObject(new MessageEvent() { Message = "Helloworld" }, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            gameStartEventMessage = gameStartEventMessage.Replace('\"', '\'');

            List<String> userPushTokens = new List<string>() { "1205536756157623" };
            pushService.SendPushMessageToGroup(gameStartEventMessage, userPushTokens);
        }
    }
}
