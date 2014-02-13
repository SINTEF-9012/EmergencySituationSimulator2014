using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;
using EmergencySituationSimulator2013.HereAPI;
using EmergencySituationSimulator2013.Model;
using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BridgeSimpleSimulator");

            // Load settings
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            // Setting up the Here API
            HereConfig.AppId = appSettings["HereAppId"];
            HereConfig.AppCode = appSettings["HereAppCode"];

            // Retreive the GPS position for the address
            var placeInfos = new HereGeocoder(appSettings["place"]);


            // Setting up the Oracle
            Oracle.Generator = Random.fromSeed(appSettings["seed"]);
            Oracle.Center = placeInfos.Location;
            double.TryParse(appSettings["areaRadius"], out Oracle.AreaRadius);
            Oracle.Center = Oracle.CreateLocation(Oracle.AreaRadius/8);
            

            /*for (var i = 0; i < 10; ++i)
            {
                new PoliceCar();
            }
            new FireTruck();
            new Patient();
            new Zombie();*/

            int time = 250;

            


            var transmission = new Transmission(appSettings["connection"], appSettings["senderID"]);


            var center = Oracle.CreateLocation(Oracle.AreaRadius / 6);
            var centerAverage = new Location(center);

            var canards = new List<Tuple<Patient, Motion>>();
            

            for(var ii = 0; ii < 20; ++ii)
            {
                var p = new Patient
                    {
                        Location = Oracle.CreateLocation(Oracle.AreaRadius/8, center)
                    };
                p.Hit();
                p.AutomaticTriage();

                centerAverage.lat = (centerAverage.lat + p.Location.lat) / 2;
                centerAverage.lng = (centerAverage.lng + p.Location.lng) / 2;

                var motion = new Motion(0.4);

                canards.Add(new Tuple<Patient, Motion>(p, motion));

                
            }

            var lapins = new List<Tuple<FireTruck, LocationPilot>>();

            for (var i = 0; i < 3; ++i)
            {
                var fireTruck = new FireTruck();
                fireTruck.Name = "SuperCamion";
                var path = new HereRoute(Oracle.CreateLocation(), centerAverage);

                var fireTruckPilot = new LocationPilot(fireTruck, path.Route, path.Speeds)
                    {
                        MaxSpeed = 300.5
                    };


               lapins.Add(new Tuple<FireTruck, LocationPilot>(fireTruck, fireTruckPilot));
            }

            var masterVisitor = new MasterVisitor();
            var textDebugVisitor = new TextDebugVisitor();

            var lapin = new Timer(delegate
                {
                    foreach (var canard in canards)
                    {
                        var m = canard.Item2.Move(Oracle.Generator, 0.06);
                        canard.Item1.Location.Move(m.Item1, m.Item2);
                    }

                    foreach (var lapinou in lapins)
                    {
                        if (!lapinou.Item2.AtDestination)
                        {
                            lapinou.Item2.Tick(0.06);
                            Console.WriteLine(lapinou.Item1.Location + " - " + lapinou.Item2.CurrentSpeed);
                        }
                    }

                    masterVisitor.StartTransaction();
                    Entity.VisitAll(masterVisitor);
                    masterVisitor.FinishTransaction();
                    transmission.Send(masterVisitor.Transaction);
                    Console.WriteLine("sent :-)");
                    Entity.VisitAll(textDebugVisitor);
                }, null, 0, time);

            
            Thread.Sleep(Timeout.Infinite);
            return;
            // Load settings
            /*NameValueCollection appSettings = ConfigurationManager.AppSettings;

            int areaRadius, patientsNumber, patientsGroupsNumber, resourcesNumber, resourcesGroupsNumber;

            int.TryParse(appSettings["areaRadius"], out areaRadius);
            int.TryParse(appSettings["patientsNumber"], out patientsNumber);
            int.TryParse(appSettings["patientsGroupsNumber"], out patientsGroupsNumber);
            int.TryParse(appSettings["resourcesNumber"], out resourcesNumber);
            int.TryParse(appSettings["resourcesGroupsNumber"], out resourcesGroupsNumber);

            Location center = Location.fromString(appSettings["locationCenter"]);

            Random random = Random.fromSeed(appSettings["seed"]);

            double patientsSpeedRatio, resourcesSpeedRatio, speedInsideGroup;
            double.TryParse(appSettings["patientsSpeedRaio"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out patientsSpeedRatio);
            double.TryParse(appSettings["resourcesSpeedRaio"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out resourcesSpeedRatio);
            double.TryParse(appSettings["speedInsideGroup"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out speedInsideGroup);

            // Create entities

            var patients = new List<OLdEntity>(patientsNumber);
            //var resources = new List<OLdEntity>(resourcesNumber);
            var groups = new List<Group>(patientsGroupsNumber + resourcesGroupsNumber);

            Group currentGroup = null;
            Location currentLocation = null;
            int nbGroup = 0;

            for (int i = 0; i < patientsNumber; ++i)
            {
                if (currentGroup == null ||
                    (nbGroup < patientsGroupsNumber && random.Next(patientsNumber/patientsGroupsNumber) == 1))
                {
                    currentGroup = new Group(patientsSpeedRatio);
                    currentLocation = new Location()
                    {
                        lat = center.lat,
                        lng = center.lng
                    };

                    currentLocation.Move(random.NextDouble()*360.0, random.NextGaussianDouble(areaRadius/2, areaRadius/4));

                    groups.Add(currentGroup);
                }

                var patient = new OLdEntity(Math.Abs(random.NextGaussianDouble(speedInsideGroup, speedInsideGroup)));

                // Place the patient on the area
                patient.location.lat = currentLocation.lat;

                patient.location.lng = currentLocation.lng;
                patient.location.Move(random.NextDouble()*360.0, random.NextGaussianDouble(currentGroup.entities.Count*2+5, currentGroup.entities.Count+10));

                currentGroup.AddEntity(patient);
                patients.Add(patient);
            }

            // Start the communication system
            var transmission = new Transmission(appSettings["connection"], appSettings["senderID"]);
            transmission.init(patients);

            // Start the main loop

            var timer = new Timer(delegate
            {
                foreach (var group in groups)
                {
                    group.Move(random, 0.08);
                }
                foreach (OLdEntity patient in patients)
                {
                    patient.Move(random, 0.08);
                }

                transmission.update(patients);
            }, null, 0, 300);

            

            // Commit when the user type enter
            do
            {
                
                transmission.update(patients);
            } while (!Console.ReadLine().Contains("exit"));*/
        }
    }
}