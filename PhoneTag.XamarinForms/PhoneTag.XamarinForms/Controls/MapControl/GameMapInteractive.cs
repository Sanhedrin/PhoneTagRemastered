using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Controls.MapControl
{
    /// <summary>
    /// A custom control representing an interactive map that can be put as a view on a page.
    /// The map is initialized to a given play area and is locked to it, allowing players to view the game
    /// area, but not stray away from it to keep ease of use.
    /// </summary>
    class GameMapInteractive : GameMapDisplay
    {
        public GameMapInteractive(Position i_GameLocation, double i_GameRadius, double i_ZoomRadius) : base(i_GameLocation, i_GameRadius, i_ZoomRadius)
        {
        }
    }
}
