namespace EmergencySituationSimulator2014.HereAPI
{
    class ManeuverType
    {
        public double Length { get; set; }
        public double TravelTime { get; set; }
        public GeoCoordinateType Position { get; set; }
        public string Instruction { get; set; }
    }
}
