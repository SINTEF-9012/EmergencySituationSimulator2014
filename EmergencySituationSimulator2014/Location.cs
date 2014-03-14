using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmergencySituationSimulator2014
{
    public class Location
    {
        public double lat;
        public double lng;

        public Location()
        {
            
        }

        public Location(Location copy)
        {
            lat = copy.lat;
            lng = copy.lng;
        }
        
		public Location(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        public Location(string location)
        {
            string[] tab = location.Split(',');
            
            double.TryParse(tab[0], NumberStyles.Any, CultureInfo.InvariantCulture, out this.lat);
            double.TryParse(tab[1], NumberStyles.Any, CultureInfo.InvariantCulture, out this.lng);
        }

        public override string ToString()
        {
            return "" + lat + " " + lng;
        }

        public void Move(double bearing, double distance)
        {
            // formula from http://stackoverflow.com/questions/9550282/moving-gps-coordinate
            // and http://www.movable-type.co.uk/scripts/latlong.html
            
            const double R = 6378137.0;
            const double toRadian = (Math.PI/180.0);
            double cdr = Math.Cos(distance/R);
            double sdr = Math.Sin(distance/R);

            double lat = this.lat*toRadian;
            double lng = this.lng*toRadian;
            bearing *= toRadian;

            double lat2 = Math.Asin(Math.Sin(lat)*cdr +
                                    Math.Cos(lat)*sdr*Math.Cos(bearing));
            double lng2 = lng + Math.Atan2(Math.Sin(bearing)*sdr*Math.Cos(lat),
                cdr - Math.Sin(lat)*Math.Sin(lat2));

            lng2 = (lng2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;  // normalise to -180..+180º

            lat2 = lat2*180.0/Math.PI;
            lng2 = lng2*180.0/Math.PI;

            //Console.WriteLine("=> " + (this.lat-lat2) + " " + (this.lng-lng2));

            this.lat = lat2;
            this.lng = lng2;
        }

        /**
         * Distance between two GPS points.
         *
         * Adapted algorithm from JOSM, an OpenStreetMap editor.
         *
         * Use Haversine method behind.
         *
         * @return Distance
         */
        public double DistanceTo(Location point)
        {
            const double R = 6378137.0;
            const double toRadian = (Math.PI / 180.0);

            double sinHalfLat = Math.Sin(toRadian * (point.lat - this.lat) / 2);
            double sinHalfLon = Math.Sin(toRadian * (point.lng - this.lng) / 2);
            double d = 2
                            * R
                            * Math.Asin(Math.Sqrt(sinHalfLat * sinHalfLat
                                            + Math.Cos(this.lat * toRadian)
                                            * Math.Cos(point.lat * toRadian) * sinHalfLon
                                            * sinHalfLon));
            // For points opposite to each other on the sphere,
            // rounding errors could make the argument of asin greater than 1
            // (This should almost never happen.)
            if (double.IsNaN(d))
            {
                Console.WriteLine("Error: NaN in greatCircleDistance");
                d = Math.PI * R;
            }
            
            return d;
        }

        /**
         * Angle of a 3 points curve.
         *
         * Using Al-Kashi theorem
         *
         * @return Angle in degree
         */
        public static double Angle(Location A, Location B, Location C)
        {
            const double toDegree = (180.0/Math.PI);

            double lA = A.DistanceTo(B);
            double lB = B.DistanceTo(C);
            double lC = C.DistanceTo(A);

            return 180 - toDegree * (Math.Acos((lA * lA + lB * lB - lC * lC)
                            / (2 * lA * lB)));
        }

        public override bool Equals(object obj)
        {
            var other = obj as Location;
            if (other == null)
            {
                return false;
            }
            return Math.Abs(lat - other.lat) < double.Epsilon &&
               Math.Abs(lng - other.lng) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return (int) (Math.Round(lat*10000) + Math.Round(lng*10000)*100000);
        }
    }
}