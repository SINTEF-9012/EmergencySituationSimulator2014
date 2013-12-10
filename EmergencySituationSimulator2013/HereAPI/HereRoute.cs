using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestSharp;

namespace EmergencySituationSimulator2013.HereAPI
{
    class HereRoute
    {
        private static readonly RestClient Client =
            new RestClient("http://route.cit.api.here.com");

        public static string AppId;
        public static string AppCode;

        public IRestResponse<HereResponse> Response { get; protected set; }
        public List<Location> Route { get; protected set; }
        public Dictionary<Location, double> Speeds { get; protected set; } 

        public HereRoute(Location from, Location to, bool pedestrian = false)
        {
            var request = new RestRequest("/routing/7.2/calculateroute.json",
                Method.GET);

            // Application parameters
            request.AddParameter("app_id", AppId);
            request.AddParameter("app_code", AppCode);

            // Pedestrian or car
            request.AddParameter("mode", "fastest;" +
                                         (pedestrian ? "pedestrian" : "car") +

                                         // tollroad are not funny and public transports are disabled
                                         ";traffic:disabled;tollroad:-1");

            // We just want the shapes
            request.AddParameter("routeattributes", "shape,legs");
            request.AddParameter("maneuverattributes", "travelTime");
            request.AddParameter("legattributes", "travelTime");
            request.AddParameter("linkattributes", "none");

            // First character of identifiers in uppercase
            request.AddParameter("jsonAttributes", "0");

            // And the waypoints
            request.AddParameter("waypoint0",String.Format(
                CultureInfo.InvariantCulture, "{0:F},{1:F}",
                from.lat, from.lng));
            request.AddParameter("waypoint1", String.Format(
                CultureInfo.InvariantCulture, "{0:F},{1:F}",
                to.lat, to.lng));

            // Magic
            Response = Client.Execute<HereResponse>(request);
            
            try
            {
                // Convert the response to something cool
                Route = Response.Data.Response.Route.First().Shape.Select(
                    location => new Location(location)
                ).ToList();

                Speeds = new Dictionary<Location, double>();

                foreach (RouteLegType routeLeg in Response.Data.Response.Route.First().Leg)
                {
                    foreach (ManeuverType maneuver in routeLeg.Maneuver)
                    {
                        var location = new Location
                            {
                                lat = maneuver.Position.Latitude,
                                lng = maneuver.Position.Longitude
                            };
                        var speed = maneuver.Length/maneuver.TravelTime;
                        Speeds[location] = speed;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("HereRoute doesn't work :P", e);
            }
        }
    }
}
