using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Java.Util;
using PhoneTag.SharedCodebase;
using Microsoft.CSharp;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Android.Content.PM;

namespace PhoneTag.Android
{
    [Activity(Label = "PhoneTag.Android", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]   
    public class MainActivity : Activity
    {
        Button button1;
        Button button2;
        Button button3;
        Button button4;
        Button button5;
        EditText editText1;
        IGeolocator locator = CrossGeolocator.Current;
        TextView textView1;

        private Point m_CurrentOrientation;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            button1 = FindViewById<Button>(Resource.Id.button1);
            button2 = FindViewById<Button>(Resource.Id.button2);
            button3 = FindViewById<Button>(Resource.Id.button3);
            button4 = FindViewById<Button>(Resource.Id.button4);
            button5 = FindViewById<Button>(Resource.Id.button5);
            editText1 = FindViewById<EditText>(Resource.Id.editText1);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);

            button1.Click += delegate { updatePosition(1); };
            button2.Click += delegate { updatePosition(2); };
            button3.Click += delegate { shoot(1); };
            button4.Click += delegate { shoot(2); };
            button5.Click += delegate { clear(); };

            //checkServerStatus();

            initDeviceMotionServices();

            locator.DesiredAccuracy = 1;
        }


        private void initDeviceMotionServices()
        {
            
            CrossDeviceMotion.Current.Start(DeviceMotion.Plugin.Abstractions.MotionSensorType.Compass, DeviceMotion.Plugin.Abstractions.MotionSensorDelay.Fastest);
            CrossDeviceMotion.Current.SensorValueChanged += (sender, e) => {
                m_CurrentOrientation = new Point(((MotionVector)e.Value).X, ((MotionVector)e.Value).Y);
                textView1.Text = e.Value.ToString();
                
            };
        }

        private string trimDouble(double num)
        {
            string res;
            res = num >= 0 ? "+" : "";
            res += num.ToString("N4");
            return res;
        } 

        private async void checkServerStatus()
        {
            while (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    bool result = await client.GetMethodAsync("service");

                    button1.Enabled = result;
                    button2.Enabled = result;
                    button3.Enabled = result;
                    button4.Enabled = result;
                    button5.Enabled = result;

                    if (editText1.Text.Equals("Waiting for server..."))
                    {
                        editText1.Text = "Ready";
                    }
                }
            }
        }
                
        private async void updatePosition(int i_Id)
        {
            using (HttpClient client = new HttpClient())
            {
                Position position;

                try
                {
                    position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                    dynamic playerLocation = await client.PostMethodAsync(String.Format("test/position/{0}", i_Id), new { X = position.Longitude, Y = position.Latitude });
                    editText1.Text = playerLocation.ToString();//playerLocation.X.ToString() + playerLocation.Y.ToString();
                }
                catch(Exception e)
                {
                    editText1.Text = e.Message;
                }
            }
        }

        private async void shoot(int i_Id)
        {
            using (HttpClient client = new HttpClient())
            {
                Position position;

                try
                {
                    position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                    editText1.Text = await client.PostMethodAsync(String.Format("test/shoot/{0}", i_Id), 
                        new
                        {
                            DeviceLocation = new
                            {
                                X = position.Longitude,
                                Y = position.Latitude
                            },
                            DeviceOrientation = new
                            {
                                X = m_CurrentOrientation.X,
                                Y = m_CurrentOrientation.Y
                            }
                        }) + " hit";
                }
                catch (Exception e)
                {
                    editText1.Text = e.Message;
                }
            }
        }

        private async void clear()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PostMethodAsync("test/clear");
            }
        }
    }
}

