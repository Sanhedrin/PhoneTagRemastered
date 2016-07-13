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
        /*We define these properties fully instead of using the syntactic sugar options offered by the language
        Since those create the members they access as private.
        This is a problem for us since when we override the property setter later to make it private, the
        members that property will access and the ones this property does are different, since inheriting
        classes can't access these private members.
        Basically, we do this so we can define the member as protected while still giving the property
        public access.*/
        protected double m_GameRadius;
        public double GameRadius { get { return m_GameRadius; }  set { m_GameRadius = value; } }
        protected Position m_StartLocation;
        public Position StartLocation { get { return m_StartLocation; } set { m_StartLocation = value; } }

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
