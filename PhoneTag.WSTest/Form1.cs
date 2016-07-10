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

namespace PhoneTag.WSTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
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

        private async void clearUser()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.GetMethodAsync("test/clear");

                tbResult.Text = "Cleared";
            }
        }

        private async void createUser()
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

        private async void getUser()
        { 
            UserView user = await UserView.GetUser("000000000000000");

            tbResult.Text = JsonConvert.SerializeObject(user);
        }
    }
}
