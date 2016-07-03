using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Controls
{
    public class GameMap : Map
    {
        public double MaxZoom { get; private set; }
        public double MinZoom { get; private set; }
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

            MaxZoom = i_GameRadius * 2;
            MinZoom = i_GameRadius * 2;

            StartLocation = i_GameLocation;
            GameRadius = i_GameRadius;
        }
    }
}
