using System.Collections.Generic;

namespace EmergencySituationSimulator2013.HereAPI
{
    class RouteLegType
    {
        public List<ManeuverType> Maneuver { get; set; }
        public double Length { get; set; }
        public double TravelTime { get; set; }
    }
}
