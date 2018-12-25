namespace Gaia.Core.Utils
{
    /// <summary>
    /// This is essentially a radius Vector with the equatorial radius set to that of the earth
    /// </summary>
    public struct GeoPosition
    {
        /// <summary>
        /// Radius of the earth in meters
        /// </summary>
        private const double EarthRadius = 6378000;

        private double _longitude;
        private double _latitude ;

        public double Longitude
        {
            get => _longitude;
            set => _longitude = value % 180;
        }

        public double Latitude
        {
            get => _latitude;
            set => _latitude = value % 90;
        }

        public double Altitude { get; set; }

        public double Radius => EarthRadius + Altitude;

        public GeoPosition(double latitude, double longitude, double altitude = 0)
        :this()
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        #region Operator overloads
        public static GeoPosition operator +(GeoPosition position, GeoDisplacement displacement)
        => new GeoPosition(
            position.Latitude + displacement.Zenith, 
            position.Longitude + displacement.Azimuth,
            position.Altitude + displacement.Radial);

        public static GeoPosition operator +(GeoDisplacement displacement, GeoPosition position)
        => new GeoPosition(
            position.Latitude + displacement.Zenith,
            position.Longitude + displacement.Azimuth,
            position.Altitude + displacement.Radial);
        #endregion
    }
}
