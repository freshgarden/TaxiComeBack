using System;
using System.Collections.Generic;

namespace TaxiCameBack.MapUtilities
{
    public class Util
    {
        /// <summary>
        /// Convert from geographic to geocentric latitude (radians).
        /// </summary>
        /// <returns></returns>
        public static double GeoGeocentricLatitude(double geographicLatitude)
        {
            var flattening = 1.0 / 298.257223563; // WGS84
            var f = (1.0 - flattening) * (1.0 - flattening);
            return Math.Atan((Math.Tan(geographicLatitude) * f));
        }

        /// <summary>
        /// Convert from geocentric to geographic latitude (radians)
        /// </summary>
        /// <param name="geocentricLatitude"></param>
        /// <returns></returns>
        public static double GeoGeographicLatitude(double geocentricLatitude)
        {
            const double flattening = 1.0 / 298.257223563; //WGS84
            var f = (1.0 - flattening) * (1.0 - flattening);
            return Math.Atan(Math.Tan(geocentricLatitude) / f);
        }

        public static Geographic GeoGetIntersection(Geographic geo1, Geographic geo2, Geographic geo3, Geographic geo4)
        {
            var geoCross1 = geo1.CrossNormalize(geo2);
            var geoCross3 = geo3.CrossNormalize(geo4);
            return geoCross1.CrossNormalize(geoCross3);
        }

        /// <summary>
        /// From Radians to Meters
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double GeoRadiansToMeters(double rad)
        {
            return rad * 6378137.0; // WGS84 Equatorial Radius in Meters
        }

        /// <summary>
        /// From Meters to Radians
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double GeoMetersToRadians(double m)
        {
            return m / 6378137.0; // WGS84 Equatorial Radius in Meters
        }

        /// <summary>
        /// Distance in meters from GLatLng point to GPolyline or GPolygon poly
        /// </summary>
        /// <param name="pointLatLngs"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double GeoDistanceToPolyMtrs(List<PointLatLng> pointLatLngs, PointLatLng point)
        {
            if (!IsValidatePoint(point))
                throw new ArgumentOutOfRangeException();

            var d = 999999999.0;
            var p = new Geographic(point.Latitude, point.Longitude);
            for (var i = 0; i < pointLatLngs.Count - 1; i++)
            {
                if (!IsValidatePoint(pointLatLngs[i]))
                    throw new ArgumentOutOfRangeException();
                var l1 = new Geographic(pointLatLngs[i].Latitude, pointLatLngs[i].Longitude);
                var l2 = new Geographic(pointLatLngs[i + 1].Latitude, pointLatLngs[i + 1].Longitude);
                var dp = p.DistanceToLineSegMtrs(l1, l2);
                if (dp < d)
                {
                    d = dp;
                }
            }

            return d;
        }

        public static bool IsValidatePoint(PointLatLng point)
        {
            if (point == null)
            {
                return false;
            }

            if (point.IsEmpty)
            {
                return false;
            }

            var latitude1 = Math.Abs(Math.Round(point.Latitude * 1000000));
            if (latitude1 > 90 * 1000000)
            {
                return false;
            }

            var longitude1 = Math.Abs(Math.Round(point.Longitude * 1000000));
            if (longitude1 > 180 * 1000000)
            {
                return false;
            }
            return true;
        }
    }
}
