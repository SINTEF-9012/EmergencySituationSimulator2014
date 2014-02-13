using System.Collections.Generic;

namespace EmergencySituationSimulator2014.HereAPI
{
    class RouteType
    {
        public List<string> Shape { get; set; }
        public List<RouteLegType> Leg { get; set; }
    }
}
