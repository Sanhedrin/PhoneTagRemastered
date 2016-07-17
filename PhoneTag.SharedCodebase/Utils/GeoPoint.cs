namespace PhoneTag.WebServices.Utils
{
    /// <summary>
    /// POCO representation of a geographical point.
    /// </summary>
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPoint(double i_Latitude, double i_Longitude)
        {
            Latitude = i_Latitude;
            Longitude = i_Longitude;
        }
    }
}