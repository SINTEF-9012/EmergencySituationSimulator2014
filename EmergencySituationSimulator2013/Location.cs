using System;
using System.Globalization;

namespace EmergencySituationSimulator2013
{
    public class Location
    {
        public double lat;
        public double lng;


        public static Location fromString(string location)
        {
            var newLocation = new Location();

            string[] tab = location.Split(',');


            double.TryParse(tab[0], NumberStyles.Any, CultureInfo.InvariantCulture, out newLocation.lat);
            double.TryParse(tab[1], NumberStyles.Any, CultureInfo.InvariantCulture, out newLocation.lng);

            return newLocation;
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
    }
}