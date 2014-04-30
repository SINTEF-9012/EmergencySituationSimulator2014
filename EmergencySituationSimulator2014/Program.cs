using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using EmergencySituationSimulator2014.HereAPI;
using EmergencySituationSimulator2014.Model;
using EmergencySituationSimulator2014.Visitors;
using ThingModel;
using ThingModel.WebSockets;
using Timer = System.Timers.Timer;

namespace EmergencySituationSimulator2014
{
    internal class Program
    {
        private static void MainOld(/*string[] args*/)
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

	        Warehouse wharehouse = new Warehouse();

	        var thingModelClient = new Client(appSettings["senderID"], appSettings["connection"], wharehouse);

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

            var thingModelVisitor = new ThingModelVisitor(wharehouse);
            var textDebugVisitor = new TextDebugVisitor();

	        var timer = new Timer();
	        timer.Interval = time*3;
			// ReSharper disable once ObjectCreationAsStatement
	        timer.Elapsed += delegate
	        {
		        lock (wharehouse)
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

			        Entity.VisitAll(thingModelVisitor);
			        thingModelClient.Send();
			        Console.WriteLine("sent :-)");
			        Entity.VisitAll(textDebugVisitor);
		        }
	        };

	        timer.Enabled = true;
			timer.Start();
            
            Thread.Sleep(Timeout.Infinite);
        }
    }
}