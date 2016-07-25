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

[assembly: ExportRenderer(typeof(GameMapInteractive), typeof(GameMapInteractiveRenderer))]
namespace PhoneTag.XamarinForms.Droid
{
    /// <summary>
    /// Custom renderer for the game map control.
    /// </summary>
    class GameMapInteractiveRenderer : GameMapDisplayRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            ((GameMapInteractive)e.NewElement).PlayerLocationMarkersUpdated += GameMapInteractiveRenderer_PlayerLocationMarkersUpdated; ;
        }

        private void GameMapInteractiveRenderer_PlayerLocationMarkersUpdated(object sender, PlayerLocationMarkerUpdateEventArgs e)
        {
            updatePlayerLocations(e.PlayerLocations);
        }

        //Adds the player location markers to the map.
        private async Task updatePlayerLocations(List<Tuple<PlayerAllegiance, GeoPoint>> i_PlayerLocations)
        {
            while (m_MapView == null)
            {
                await Task.Delay(10);
            }

            markPlayArea(new Position(m_GameLocation.Latitude, m_GameLocation.Longitude), m_GameRadius, false);

            foreach (Tuple<PlayerAllegiance, GeoPoint> marker in i_PlayerLocations)
            {
                MarkerOptions markerOptions = getMarkerOptionsFor(marker);

                m_MapView.AddMarker(markerOptions);
            }
        }

        //Gets the relevant marker options for the given player information.
        private MarkerOptions getMarkerOptionsFor(Tuple<PlayerAllegiance, GeoPoint> i_PlayerMarker)
        {
            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.Draggable(false);
            markerOptions.SetPosition(new LatLng(i_PlayerMarker.Item2.Latitude, i_PlayerMarker.Item2.Longitude));
            m_MapView.UiSettings.MapToolbarEnabled = false;

            BitmapDescriptor markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueMagenta);
            switch (i_PlayerMarker.Item1)
            {
                case PlayerAllegiance.Ally:
                    markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen);
                    markerOptions.SetTitle("Ally");
                    break;
                case PlayerAllegiance.Enemy:
                    markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed);
                    markerOptions.SetTitle("Enemy");
                    break;
                case PlayerAllegiance.Self:
                    markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue);
                    markerOptions.SetTitle("Me");
                    break;
                case PlayerAllegiance.Special:
                    markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow);
                    markerOptions.SetTitle("Special Ally");
                    break;
                case PlayerAllegiance.SpecialSelf:
                    markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueOrange);
                    markerOptions.SetTitle("Special Me");
                    break;
            }

            markerOptions.SetIcon(markerColor);

            return markerOptions;
        }
    }
}