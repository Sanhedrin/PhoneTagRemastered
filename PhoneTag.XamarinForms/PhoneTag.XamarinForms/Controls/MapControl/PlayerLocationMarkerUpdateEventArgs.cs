using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls.MapControl
{
    public class PlayerLocationMarkerUpdateEventArgs : EventArgs
    {
        public List<Tuple<PlayerAllegiance, String, GeoPoint>> PlayerLocations { get; set; }

        public PlayerLocationMarkerUpdateEventArgs(List<Tuple<PlayerAllegiance, String, GeoPoint>> i_PlayerLocations)
        {
            PlayerLocations = i_PlayerLocations;
        }
    }
}
