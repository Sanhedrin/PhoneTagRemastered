using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.POCOs
{
    public class LocationUpdateInfo
    {
        public String FBID { get; set; }
        public String Username { get; set; }
        public GeoPoint Location { get; set; }

        public LocationUpdateInfo(String i_FBID, String i_Username, GeoPoint i_Location)
        {
            FBID = i_FBID;
            Username = i_Username;
            Location = i_Location;
        }
    }
}
