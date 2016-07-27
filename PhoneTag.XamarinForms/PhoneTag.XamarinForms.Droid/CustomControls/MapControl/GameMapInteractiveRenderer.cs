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
        private async Task updatePlayerLocations(List<Tuple<PlayerAllegiance, String, GeoPoint>> i_PlayerLocations)
        {
            while (m_MapView == null)
            {
                await Task.Delay(10);
            }

            markPlayArea(new Position(m_GameLocation.Latitude, m_GameLocation.Longitude), m_GameRadius, false);

            foreach (Tuple<PlayerAllegiance, String, GeoPoint> marker in i_PlayerLocations)
            {
                MarkerOptions markerOptions = getMarkerOptionsFor(marker);

                m_MapView.AddMarker(markerOptions);
            }
        }

        //Gets the relevant marker options for the given player information.
        private MarkerOptions getMarkerOptionsFor(Tuple<PlayerAllegiance, String, GeoPoint> i_PlayerMarker)
        {
            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.Draggable(false);
            markerOptions.SetPosition(new LatLng(i_PlayerMarker.Item3.Latitude, i_PlayerMarker.Item3.Longitude));
            markerOptions.SetTitle(i_PlayerMarker.Item2);
            m_MapView.UiSettings.MapToolbarEnabled = false;

            BitmapDescriptor markerColor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueMagenta);
            switch (i_PlayerMarker.Item1)
            {
                case PlayerAllegiance.Ally:
                    markerColor = BitmapDescriptorFactory.FromResource(Resource.Drawable.heart);
                    break;
                case PlayerAllegiance.Enemy:
                    markerColor = BitmapDescriptorFactory.FromResource(Resource.Drawable.attack);
                    break;
                case PlayerAllegiance.Self:
                    markerColor = BitmapDescriptorFactory.FromResource(Resource.Drawable.myself);
                    break;
                case PlayerAllegiance.Special:
                    markerColor = BitmapDescriptorFactory.FromResource(Resource.Drawable.protect);
                    break;
                case PlayerAllegiance.SpecialSelf:
                    markerColor = BitmapDescriptorFactory.FromResource(Resource.Drawable.protect_me);
                    break;
            }

            markerOptions.SetIcon(markerColor);

            return markerOptions;
        }
    }
}