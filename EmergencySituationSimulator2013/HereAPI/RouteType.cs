using System.Collections.Generic;

namespace EmergencySituationSimulator2013.HereAPI
{
    class RouteType
    {
        public List<string> Shape { get; set; }
        public List<RouteLegType> Leg { get; set; }
    }
}
