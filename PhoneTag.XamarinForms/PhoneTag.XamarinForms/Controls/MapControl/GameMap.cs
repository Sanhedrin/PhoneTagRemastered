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
    /// A custom control representing an interactive map that can be put as a view on a page.
    /// The map is initialized to a given play area and is locked to it, allowing players to view the game
    /// area, but not stray away from it to keep ease of use.
    /// </summary>
    public class GameMap : Map
    {
        public double GameRadius { get; private set; }
        public Position StartLocation { get; private set; }

        public GameMap(Position i_GameLocation, double i_GameRadius, double i_ZoomRadius) : 
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
