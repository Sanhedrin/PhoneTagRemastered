using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Utils
{
    /// <summary>
    /// Provides basic geographical location operations.
    /// </summary>
    public static class GeoUtils
    {
        private const double k_EarthRadius = 6371e3; // metres

        /// <summary>
        /// Gets the distance in kilometers between 2 geographical points.
        /// </summary>
        public static double GetDistanceBetween(GeoPoint i_Location1, GeoPoint i_Location2)
        {
            double lat1Rad = ToRadians(i_Location1.Latitude);
            double lat2Rad = ToRadians(i_Location2.Latitude);
            double latDeltaRad = ToRadians(i_Location1.Latitude - i_Location2.Latitude);
            double lonDeltaRad = ToRadians(i_Location1.Longitude - i_Location2.Longitude);

            double calculatedValue1 = Math.Sin(latDeltaRad / 2) * Math.Sin(latDeltaRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(lonDeltaRad / 2) * Math.Sin(lonDeltaRad / 2);
            double calculatedValue2 = 2 * Math.Atan2(Math.Sqrt(calculatedValue1), Math.Sqrt(1 - calculatedValue1));

            double distance = (k_EarthRadius * calculatedValue2) / 1000; //Based on earth radius, in km

            return distance;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <returns></returns>
        public static double ToRadians(double i_Degree)
        {
            return i_Degree * Math.PI / 180;
        }
    }
}
