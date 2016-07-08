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
    /// A specialized Game Map that allows setting the play area prior to starting a new game.
    /// </summary>
    public class GameMapSetup : GameMap
    {
        //We override the GameRadius and StartLocation properties of the game map since the setup map
        //allows setting to these values, whereas the play map doesn't.
        public new double GameRadius { get; set; }
        public new Position StartLocation { get; set; }

        public GameMapSetup(Position i_GameLocation, double i_GameRadius, double i_ZoomRadius) :
            base(i_GameLocation, i_GameRadius, i_ZoomRadius)
        {
        }
    }
}
