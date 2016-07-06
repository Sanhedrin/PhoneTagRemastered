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

using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;

using PhoneTag.XamarinForms.Droid;
using PhoneTag.XamarinForms.Controls;
using System.ComponentModel;
using Android.Gms.Maps;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(GameMap), typeof(GameMapRenderer))]
namespace PhoneTag.XamarinForms.Droid
{
    class GameMapRenderer : MapRenderer
    {

        private const double k_EarthRadius = 6371e3; // metres

        private LatLng m_GameLocation;
        private double m_GameRadius;
        private double m_MinZoom;
        private double m_MaxZoom;
        private double? m_InitialZoom = null;
        private GoogleMap m_MapView;

        private LatLng m_LastValidLocation;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            m_MapView = ((MapView)Control).Map;
            m_MapView.UiSettings.ZoomControlsEnabled = false;

            m_MapView.SetIndoorEnabled(true);

            m_MapView.CameraChange += CameraChanged;
        }

        public void CameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            fixCameraZoom(e.Position.Zoom);

            fixCameraPan(e.Position.Target);
        }

        private void fixCameraZoom(double i_CurrentZoom)
        {
            if (m_InitialZoom == null)
            {
                m_InitialZoom = m_MapView.CameraPosition.Zoom;
                m_MaxZoom = m_InitialZoom.Value + m_MaxZoom;
                m_MinZoom = m_InitialZoom.Value - m_MinZoom;
            }
            else if (i_CurrentZoom > m_MaxZoom)
            {
                m_MapView.AnimateCamera(CameraUpdateFactory.ZoomTo((float)m_MaxZoom));
            }
            else if (i_CurrentZoom < m_MinZoom)
            {
                m_MapView.AnimateCamera(CameraUpdateFactory.ZoomTo((float)m_MinZoom));
            }
        }

        private void fixCameraPan(LatLng i_CurrentLocation)
        {
            if (isLocationOutOfBounds(i_CurrentLocation))
            {
                m_MapView.AnimateCamera(CameraUpdateFactory.NewLatLng(m_LastValidLocation));
            }
            else
            {
                m_LastValidLocation = i_CurrentLocation;
            }
        }

        private bool isLocationOutOfBounds(LatLng i_Location)
        {
            double lat1Rad = toRadians(i_Location.Latitude);
            double lat2Rad = toRadians(m_GameLocation.Latitude);
            double latDeltaRad = toRadians(i_Location.Latitude - m_GameLocation.Latitude);
            double lonDeltaRad = toRadians(i_Location.Longitude - m_GameLocation.Longitude);

            double calculatedValue1 = Math.Sin(latDeltaRad / 2) * Math.Sin(latDeltaRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(lonDeltaRad / 2) * Math.Sin(lonDeltaRad / 2);
            double calculatedValue2 = 2 * Math.Atan2(Math.Sqrt(calculatedValue1), Math.Sqrt(1 - calculatedValue1));

            double distance = (k_EarthRadius * calculatedValue2) / 1000; //Based on earth radius, in km

            return distance > m_GameRadius;
        }

        private double toRadians(double i_Degree)
        {
            return i_Degree * Math.PI / 180;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            GameMap gameMap = (GameMap)e.NewElement;
            m_MaxZoom = gameMap.MaxZoom;
            m_MinZoom = gameMap.MinZoom;
            m_LastValidLocation = m_GameLocation = new LatLng(gameMap.StartLocation.Latitude, gameMap.StartLocation.Longitude);
            m_GameRadius = gameMap.GameRadius;

            markPlayArea(gameMap.StartLocation, gameMap.GameRadius);
        }

        private async void markPlayArea(Position i_GameLocation, double i_GameRadius)
        {
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(new LatLng(i_GameLocation.Latitude, i_GameLocation.Longitude));
            circleOptions.InvokeRadius(i_GameRadius * 1000); //Meters to kilometers
            circleOptions.InvokeFillColor(Android.Graphics.Color.Argb(50, 255, 0, 0));
            circleOptions.InvokeStrokeWidth(3);

            while (m_MapView == null)
            {
                await Task.Delay(10);
            }

            Circle newCircle = m_MapView.AddCircle(circleOptions);
            newCircle.Visible = true;
        }
    }
}