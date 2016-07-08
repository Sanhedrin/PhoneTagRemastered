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
using PhoneTag.XamarinForms.Droid.CustomControls.MapControl;
using Xamarin.Forms;
using Android.Gms.Maps;
using PhoneTag.XamarinForms.Controls.MapControl;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using PhoneTag.SharedCodebase.Utils;

[assembly: ExportRenderer(typeof(GameMapSetup), typeof(GameMapSetupRenderer))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.MapControl
{
    class GameMapSetupRenderer : GameMapRenderer
    {
        public GameMapSetupRenderer() : base()
        {
            setupGestures();
            setupReturnValues();
        }

        //If the area doesn't change before selection is done, the default area is chosen.
        private async void setupReturnValues()
        {
            while (m_MapView == null || m_GameMap == null) { await Task.Delay(100); }

            ((GameMapSetup)m_GameMap).StartLocation = m_GameMap.StartLocation;
            ((GameMapSetup)m_GameMap).GameRadius = m_GameMap.GameRadius;
        }

        private async void setupGestures()
        {
            while (m_MapView == null || m_GameMap == null) { await Task.Delay(100); }
            
            m_MapView.MapClick += GameMap_MapClick;
        }

        //Clicking on the map places a new game area indicator on the clicked location that has the size
        //of the viewable area on the map.
        private void GameMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            Position location = new Position(e.Point.Latitude, e.Point.Longitude);
            double distance = GeoUtils.GetDistanceBetween(
                new GeoPoint(0, m_MapView.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude),
                new GeoPoint(0, m_MapView.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude));
            double radius = distance / 2;

            ((GameMapSetup)m_GameMap).StartLocation = location;
            ((GameMapSetup)m_GameMap).GameRadius = radius / 2;

            setupMap(location, radius / 2);
        }

        /// <summary>
        /// Handles changes to the visible area on the map.
        /// We override this to disable the snap backs from moving the camera away from the play area.
        /// </summary>
        public override void CameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
        {
        }
    }
}