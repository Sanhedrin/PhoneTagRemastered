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
using PhoneTag.XamarinForms.Controls.MapControl;
using System.ComponentModel;
using Android.Gms.Maps;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using PhoneTag.SharedCodebase.Utils;

[assembly: ExportRenderer(typeof(GameMapDisplay), typeof(GameMapDisplayRenderer))]
namespace PhoneTag.XamarinForms.Droid
{
    /// <summary>
    /// Custom renderer for the game map control.
    /// </summary>
    class GameMapDisplayRenderer : MapRenderer
    {
        protected LatLng m_GameLocation;
        protected double m_GameRadius;

        private double m_MinZoom;
        private double m_MaxZoom;
        private double? m_InitialZoom = null;
        protected GoogleMap m_MapView;
        protected GameMapDisplay m_GameMap;

        protected float? m_LastZoom = null;

        private bool m_MapControlReady = false;

        private LatLng m_LastValidLocation;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            m_MapView = ((MapView)Control).Map;
            setupMapControl();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            m_GameMap = (GameMapDisplay)e.NewElement;
            Position startLocation = m_GameMap.StartLocation;
            double radius = m_GameMap.GameRadius;
            setupMap(startLocation, radius);
        }

        //UI setup for the map control.
        private void setupMapControl()
        {
            if (!m_MapControlReady)
            {
                m_MapView.UiSettings.ZoomControlsEnabled = false;
                m_MapView.UiSettings.MyLocationButtonEnabled = false;

                m_MapView.SetIndoorEnabled(true);

                m_MapView.CameraChange += CameraChanged;

                m_MapControlReady = true;
            }
        }

        //Sets the view of the map to the given game area.
        protected void setupMap(Position i_Location, double i_GameRadius)
        {
            m_LastValidLocation = m_GameLocation = new LatLng(i_Location.Latitude, i_Location.Longitude);
            m_GameRadius = i_GameRadius;

            markPlayArea(i_Location, i_GameRadius, true);
        }

        /// <summary>
        /// Handles changes to the visible area on the map.
        /// We'll ensure that it doesn't go out of range, and if it does, move the view back to the
        /// game area.
        /// </summary>
        public virtual void CameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            fixCameraZoom(e.Position.Zoom);

            fixCameraPan(e.Position.Target);
        }

        private void fixCameraZoom(double i_CurrentZoom)
        {
            if (m_MapView != null)
            {
                if (m_InitialZoom == null)
                {
                    m_InitialZoom = m_MapView.CameraPosition.Zoom;
                    m_MaxZoom = 19.5;
                    m_MinZoom = m_InitialZoom.Value - m_InitialZoom.Value * 0.05;
                }
                else if (i_CurrentZoom > m_MaxZoom)
                {
                    m_MapView.AnimateCamera(CameraUpdateFactory.ZoomTo((float)m_MaxZoom - 0.01f));
                }
                else if (i_CurrentZoom < m_MinZoom)
                {
                    m_MapView.AnimateCamera(CameraUpdateFactory.ZoomTo((float)m_MinZoom + 0.01f));
                }
            }

            m_LastZoom = (float)i_CurrentZoom;
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

        protected async Task markPlayArea(Position i_GameLocation, double i_GameRadius, bool i_CenterOnPosition)
        {
            if (i_GameLocation != null)
            {
                while (m_MapView == null)
                {
                    await Task.Delay(10);
                }

                m_MapView.Clear();

                CircleOptions circleOptions = new CircleOptions();
                LatLng gameLocation = new LatLng(i_GameLocation.Latitude, i_GameLocation.Longitude);
                circleOptions.InvokeCenter(gameLocation);
                circleOptions.InvokeRadius(i_GameRadius * 1000); //Meters to kilometers
                circleOptions.InvokeFillColor(Android.Graphics.Color.Argb(50, 255, 0, 0));
                circleOptions.InvokeStrokeWidth(3);
                Circle newCircle = m_MapView.AddCircle(circleOptions);
                newCircle.Visible = true;

                m_LastValidLocation = gameLocation;

                if (i_CenterOnPosition)
                {
                    if (m_LastZoom.HasValue)
                    {
                        m_MapView.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(m_LastValidLocation, m_LastZoom.Value));
                    }
                    else
                    {
                        m_MapView.AnimateCamera(CameraUpdateFactory.NewLatLng(m_LastValidLocation));
                    }
                }
            }
        }

        private bool isLocationOutOfBounds(LatLng i_Location)
        {
            GeoPoint currentLocation = new GeoPoint(i_Location.Latitude, i_Location.Longitude);
            GeoPoint gameLocation = new GeoPoint(m_GameLocation.Latitude, m_GameLocation.Longitude);

            double distance = GeoUtils.GetDistanceBetween(currentLocation, gameLocation);

            return distance > m_GameRadius;
        }
    }
}