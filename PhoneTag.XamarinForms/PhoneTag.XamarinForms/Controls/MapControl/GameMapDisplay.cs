using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Controls.MapControl
{
    /// <summary>
    /// A specialized game map that only allows to view the play area and doesn't actually interact
    /// with the player.
    /// </summary>
    public class GameMapDisplay : Map
    {
        public double GameRadius { get; private set; }
        public Position StartLocation { get; private set; }

        public GameMapDisplay(Position i_GameLocation, double i_GameRadius, double i_ZoomRadius) : 
            base(MapSpan.FromCenterAndRadius(i_GameLocation, Distance.FromKilometers(i_ZoomRadius)).WithZoom(2))
        {
            IsShowingUser = true;
            HeightRequest = CrossScreen.Current.Size.Height * 2 / 5;
            WidthRequest = CrossScreen.Current.Size.Width;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;

            StartLocation = i_GameLocation;
            GameRadius = i_GameRadius;
        }
    }
}
