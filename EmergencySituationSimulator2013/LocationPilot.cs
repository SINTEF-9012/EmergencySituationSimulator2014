using System.Collections.Generic;
using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013
{
    class LocationPilot
    {
        protected List<Location> Route;

        public List<Entity> Passengers { get; protected set; }

        // m/s
        public double MaxSpeed;

        // m/s too
        public double CurrentSpeed { get; protected set; }

        public bool AtDestination { get; protected set; }

        // Variables for the pilot
        protected Location PreviousWayPoint;

        protected Location NextWayPoint;

        // In meters
        // The distance between the previous waypoint
        // and the next waypoint
        protected double SegmentLength = 1;

        // In meters
        protected double CurrentDistance = 0;

        // Current segment inde
        protected int SegmentIndex = 0;

        public LocationPilot(List<Location> route, Entity firstPassenger = null)
        {
            Route = route;
            Passengers = new List<Entity>();

            if (firstPassenger != null)
            {
                Passengers.Add(firstPassenger);
            }

            NextSegment();
        }

        protected void NextSegment()
        {
            if (SegmentIndex >= Route.Count-1)
            {
                CurrentDistance = SegmentLength;
                AtDestination = true;
            }
            else
            {
                PreviousWayPoint = Route[SegmentIndex];
                NextWayPoint = Route[++SegmentIndex];

                CurrentDistance = 0.0;

                SegmentLength = PreviousWayPoint.DistanceTo(NextWayPoint);    
            }
        }
        
        public void Tick(double time)
        {
            // Very quick implementation
            CurrentSpeed = MaxSpeed;

            CurrentDistance += CurrentSpeed*time;

            if (CurrentDistance > SegmentLength)
            {
                NextSegment();
            }

            ApplyCurrentLocation();
        }

        public Location CurrentLocation()
        {
            var factor = CurrentDistance / SegmentLength;

            return new Location
                {
                    lat = PreviousWayPoint.lat + factor * (NextWayPoint.lat - PreviousWayPoint.lat),
                    lng = PreviousWayPoint.lng + factor * (NextWayPoint.lng - PreviousWayPoint.lng)
                };
        }

        public void ApplyCurrentLocation()
        {
            var location = CurrentLocation();
            foreach (var passenger in Passengers)
            {
                passenger.Location.lat = location.lat;
                passenger.Location.lng = location.lng;
            }
        }
    }
}
