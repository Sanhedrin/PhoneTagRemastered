using PhoneTag.SharedCodebase;
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
using PhoneTag.SharedCodebase.Views;
using Newtonsoft.Json.Linq;
using com.shephertz.app42.paas.sdk.csharp;
using Keys = PhoneTag.SharedCodebase.Utils.Keys;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using PhoneTag.SharedCodebase.Events;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Utils;

namespace PhoneTag.WSTest
{
    public partial class Form1 : Form
    {
        private Dictionary<String, GameRoomView> m_Rooms = new Dictionary<string, GameRoomView>();

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
                //await client.GetMethodAsync("test/clear");

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

        private void buttonEitanJoinRoom_Click(object sender, EventArgs e)
        {
            joinRoom(buttonEitanJoinRoom, "10209125269876338");
        }

        private async Task joinRoom(Button i_Button, String i_FBID)
        {
            i_Button.Enabled = false;

            if (m_Rooms.ContainsKey(i_FBID))
            {
                await m_Rooms[i_FBID].LeaveRoom(i_FBID);

                m_Rooms.Remove(i_FBID);
                i_Button.Text = "Join Room";
            }
            else
            {
                List<String> roomIds = await GameRoomView.GetAllRoomsInRange(new GeoPoint(0, 0), 6371);

                if (roomIds.Count > 0)
                {
                    m_Rooms.Add(i_FBID, await GameRoomView.GetRoom(roomIds[0]));

                    m_Rooms[i_FBID].JoinRoom(i_FBID);

                    i_Button.Text = "Leave Room";
                }
                else
                {
                    MessageBox.Show("No rooms found");
                }
            }

            i_Button.Enabled = true;
        }

        private void buttonEitanReady_Click(object sender, EventArgs e)
        {
            ready(buttonEitanReadyRoom, "10209125269876338");
        }

        private async Task ready(Button i_Button, String i_FBID)
        {
            i_Button.Enabled = false;

            if (m_Rooms.ContainsKey(i_FBID))
            {
                UserView user = await UserView.GetUser(i_FBID);

                await user.PlayerSetReady(!user.IsReady);
            }
            else
            {
                MessageBox.Show("Not in a room");
            }

            i_Button.Enabled = true;
        }

        private void buttonDimaJoinRoom_Click(object sender, EventArgs e)
        {
            joinRoom(buttonDimaJoinRoom, "10154417504178701");
        }

        private void buttonDimaReadyRoom_Click(object sender, EventArgs e)
        {
            ready(buttonDimaReadyRoom, "10154417504178701");
        }
    }
}
