using System;

namespace Gaia.Core.Utils
{
    /// <summary>
    /// Represents Geodesic displacement from a Geo point, to another Geo point.
    /// </summary>
    public struct GeoDisplacement
    {
        private double _azimuth;
        private double _zenith;

        /// <summary>
        /// Radial displacement or altitude displacement
        /// </summary>
        public double Radial { get; set; }

        /// <summary>
        /// Gives the amount of 'azimuth angular displacement' in degrees.
        /// </summary>
        public double Azimuth
        {
            get => _azimuth;
            set => _azimuth = value % 180;
        }

        /// <summary>
        /// Gives the amount of 'zenith angular displacement' in degrees
        /// </summary>
        public double Zenith
        {
            get => _zenith;
            set => _zenith = value % 90;
        }

        public GeoDisplacement(double azimuth, double zenith, double radial = 0)
        : this()
        {
            Radial = radial;
            Azimuth = azimuth;
            Zenith = zenith;
        }
    }
}
