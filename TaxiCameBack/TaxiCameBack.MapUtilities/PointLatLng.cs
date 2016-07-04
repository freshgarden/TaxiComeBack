using System;
using System.Globalization;

namespace TaxiCameBack.MapUtilities
{
    /// <summary>
    /// the point of coordinates
    /// </summary>
    [Serializable]
    public class PointLatLng
    {
        private double _latitude;
        private double _longitude;

        private bool _notEmpty;

        public PointLatLng(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
            _notEmpty = true;
        }

        /// <summary>
        /// returns true if coordinates wasn't assigned
        /// </summary>
        public bool IsEmpty => !_notEmpty;

        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
                _notEmpty = true;
            }
        }

        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
                _notEmpty = true;
            }
        }

        public override int GetHashCode()
        {
            return (Longitude.GetHashCode() ^ Latitude.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Latitude={0}, Longitude={1}}}", Latitude, Longitude);
        }
    }
}
