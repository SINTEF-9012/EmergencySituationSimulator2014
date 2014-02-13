namespace EmergencySituationSimulator2013
{
    public class AverageLocation
    {
        protected int Cpt;

        public Location Location { get; protected set; }

        public AverageLocation()
        {
            Reset();
        }

        public void Reset()
        {
            Cpt = 0;
            Location = new Location();
        }
        
        public void AddLocation(Location location)
        {
            Location.lat = (Location.lat*Cpt + location.lat)/(Cpt + 1);
            Location.lng = (Location.lng*Cpt + location.lng)/(Cpt + 1);

            ++Cpt;
        }
    }
}