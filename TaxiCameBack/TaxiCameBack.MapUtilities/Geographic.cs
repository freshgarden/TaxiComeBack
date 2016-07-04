using System;

namespace TaxiCameBack.MapUtilities
{
    public class Geographic
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        /// <summary>
        /// Construct a bdccGeo from its latitude and longitude in degrees
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        public Geographic(double lat, double lon)
        {
            if (!Util.IsValidatePoint(new PointLatLng(lat, lon)))
                throw new ArgumentOutOfRangeException();
            var theta = (lon * Math.PI / 180.0);
            var rlat = Util.GeoGeocentricLatitude(lat * Math.PI / 180.0);
            var c = Math.Cos(rlat);
            X = c * Math.Cos(theta);
            Y = c * Math.Sin(theta);
            Z = Math.Sin(rlat);
        }

        public double GetLatitudeRadians()
        {
            return Util.GeoGeographicLatitude(Math.Atan2(Z, Math.Sqrt((X * X) + (Y * Y))));
        }

        public double GetLongitudeRadians()
        {
            return Math.Atan2(Y, X);
        }

        public double GetLatitude()
        {
            return GetLatitudeRadians() * 180.0 / Math.PI;
        }

        public double GetLongitude()
        {
            return GetLongitudeRadians() * 180.0 / Math.PI;
        }

        public double Dot(Geographic geographic)
        {
            return X * geographic.X + Y * geographic.Y + Z * geographic.Z;
        }

        public double CrossLength(Geographic geographic)
        {
            var x = Y * geographic.Z - Z * geographic.Y;
            var y = Z * geographic.X - X * geographic.Z;
            var z = X * geographic.Y - Y * geographic.X;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public Geographic Scale(double s)
        {
            var r = new Geographic(0, 0)
            {
                X = X * s,
                Y = Y * s,
                Z = Z * s
            };
            return r;
        }

        public Geographic CrossNormalize(Geographic geographic)
        {
            var x = Y * geographic.Z - Z * geographic.Y;
            var y = Z * geographic.X - X * geographic.Z;
            var z = X * geographic.Y - Y * geographic.X;
            var l = Math.Sqrt(x * x + y * y + z * z);
            var r = new Geographic(0, 0)
            {
                X = x / l,
                Y = y / l,
                Z = z / l
            };
            return r;
        }

        /// <summary>
        /// Point on opposite side of the world to this point
        /// </summary>
        /// <returns></returns>
        public Geographic Antipode()
        {
            return Scale(1.0);
        }

        /// <summary>
        /// Distance in radians from this point to point v2
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double Distance(Geographic v2)
        {
            return Math.Atan2(v2.CrossLength(this), v2.Dot(this));
        }

        /// <summary>
        /// Returns in meters the minimum of the perpendicular distance of this point from the line segment geo1-geo2
        /// and the distance from this point to the line segment ends in geo1 and geo2
        /// </summary>
        /// <param name="geo1"></param>
        /// <param name="geo2"></param>
        /// <returns></returns>
        public double DistanceToLineSegMtrs(Geographic geo1, Geographic geo2)
        {
            //point on unit sphere above origin and normal to plane of geo1,geo2
            //could be either side of the plane
            var p2 = geo1.CrossNormalize(geo2);

            // intersection of GC normal to geo1/geo2 passing through p with GC geo1/geo2
            var ip = Util.GeoGetIntersection(geo1, geo2, this, p2);

            //need to check that ip or its antipode is between p1 and p2
            var d = geo1.Distance(geo2);
            var d1P = geo1.Distance(ip);
            var d2P = geo2.Distance(ip);

            if (d >= d1P && d >= d2P)
            {
                return Util.GeoRadiansToMeters(Distance(ip));
            }
            ip = ip.Antipode();
            d1P = geo1.Distance(ip);
            d2P = geo2.Distance(ip);

            if (d >= d1P && d >= d2P)
            {
                return Util.GeoRadiansToMeters(Distance(ip));
            }
            return Util.GeoRadiansToMeters(Math.Min(geo1.Distance(this), geo2.Distance(this)));
        }
    }
}
