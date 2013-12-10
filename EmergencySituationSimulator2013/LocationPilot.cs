using System;
using System.Collections.Generic;
using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013
{
    class LocationPilot
    {
        protected List<Location> Route;
        public Dictionary<Location, double> Speeds;

        public List<Entity> Passengers { get; protected set; }

        // m/s
        public double MaxSpeed;

        // m/s too
        public double CurrentSpeed { get; protected set; }

        public bool AtDestination { get; protected set; }

        // Variables for the pilot
        protected Location PreviousWayPoint;

        protected Location NextWayPoint;

        // 1 is a 0 degree curve
        // 0 is a U-Turn
        protected double FactorNextCurve;

        // In meters
        // The distance between the previous waypoint
        // and the next waypoint
        protected double SegmentLength = 0;

        // In meters
        protected double CurrentDistance = 0;

        // Current segment inde
        protected int SegmentIndex = 0;

        public LocationPilot(Entity firstPassenger, List<Location> route, Dictionary<Location, double> speeds)
        {
            Route = route;
            Speeds = speeds;
            Passengers = new List<Entity>();

            if (firstPassenger != null)
            {
                Passengers.Add(firstPassenger);
            }

            NextSegment();

            CurrentSpeed = 0.0;

            // Default max speed (doesn't need to be used but I prefer some security)
            MaxSpeed = 20.0;
        }

        protected void ComputeFactorNextCurve()
        {
            if (SegmentIndex+1 >= Route.Count)
            {
                FactorNextCurve = 1.0;
            }
            else
            {
                var angleCurve = Location.Angle(PreviousWayPoint, NextWayPoint, Route[SegmentIndex+1]);

                // Too much simple deceleration model :-)
                var angleFactor = 1 - (angleCurve / 180.0);

                if (double.IsNaN(angleFactor))
                {
                    angleFactor = 1.0;
                }

                FactorNextCurve = angleFactor*angleFactor;
            }
            
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
                if (SegmentIndex == 0)
                {
                    CurrentSpeed = 0.0;
                    CurrentDistance = 0.0;
                    PreviousWayPoint = Route[0];
                    NextWayPoint = Route[1];
                    SegmentLength = PreviousWayPoint.DistanceTo(NextWayPoint);
                    SegmentIndex = 1;
                    ComputeFactorNextCurve();
                }
                else
                {
                    CurrentSpeed *= FactorNextCurve;

                    PreviousWayPoint = Route[SegmentIndex];
                    NextWayPoint = Route[++SegmentIndex];
                    ComputeFactorNextCurve();

                    

                    CurrentDistance = CurrentDistance - SegmentLength;

                    SegmentLength = PreviousWayPoint.DistanceTo(NextWayPoint);    
                }
               
                // Only some waypoints contains speeds informations
                if (Speeds.ContainsKey(PreviousWayPoint))
                {
                    MaxSpeed = Speeds[PreviousWayPoint]*10;
                    Console.WriteLine("Vitesseee: " + MaxSpeed);
                }
            }
        }
        
        public void Tick(double time)
        {
            double ratioAcceleration = Oracle.CreateNumber(8.0,4.0);
            if (ratioAcceleration < 0.0 || ratioAcceleration > 12.0)
            {
                ratioAcceleration = 8.0;
            }
            
            // If we can accelerate
            if (SegmentLength-CurrentDistance > 6*CurrentSpeed*(1-FactorNextCurve))
            {
                // Very simple acceleration model
                CurrentSpeed += (Math.Abs(MaxSpeed - CurrentSpeed) / ratioAcceleration) * time;
            }
            else
            {
                var maxCurveSpeed = MaxSpeed*FactorNextCurve;
                CurrentSpeed -= ((CurrentSpeed - maxCurveSpeed) / ratioAcceleration) * time;
            }

            // A negative speed isn't a solution
            if (CurrentSpeed < 0)
            {
                CurrentSpeed = 0;
            }

            CurrentDistance += CurrentSpeed*time;

            while (CurrentDistance > SegmentLength)
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
