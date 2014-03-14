using System;
using System.Collections.Generic;
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
	class ProgramReborn
	{
		private Wharehouse _wharehouse;
		private List<Tuple<Patient, Motion>> _victims;
		private List<Tuple<ThingModelEntity, LocationPilot>> _pilots;

		private static void Main()
		{
            Console.WriteLine("BridgeSimpleSimulator");
			var p = new ProgramReborn();
			p.Start();
		}

		private void Start()
		{
            // Load settings
            var appSettings = ConfigurationManager.AppSettings;

            // Setting up the Here API
            HereConfig.AppId = appSettings["HereAppId"];
            HereConfig.AppCode = appSettings["HereAppCode"];
            
			// Setting up the Oracle
            Oracle.Generator = Random.fromSeed(appSettings["seed"]);
            double.TryParse(appSettings["areaRadius"], out Oracle.AreaRadius);

			_wharehouse = new Wharehouse();
	        
			var thingModelClient = new Client(appSettings["senderID"], appSettings["connection"], _wharehouse);

            var thingModelVisitor = new ThingModelVisitor(_wharehouse);
			
			thingModelClient.Send();

			_victims = new List<Tuple<Patient, Motion>>();
			_pilots = new List<Tuple<ThingModelEntity, LocationPilot>>();

			var registerTmr = new Timer();
			registerTmr.Interval = 1000;
			registerTmr.Elapsed += (sender, args) =>
			{
				_wharehouse.Events.OnNew += GenerateVictims;
				_wharehouse.Events.OnNew += ManageRouting;
				registerTmr.Stop();
			};
			registerTmr.Start();
				

	        var timer = new Timer();
			timer.Interval = 500;
			timer.Elapsed += (sender, args) =>
			{
				foreach (var victim in _victims)
				{
					var m = victim.Item2.Move(Oracle.Generator, 0.06);
					victim.Item1.Location.Move(m.Item1, m.Item2);
				}
				
				foreach (var pilot in _pilots)
				{
					if (!pilot.Item2.AtDestination)
					{
						pilot.Item2.Tick(0.06);
					     Console.WriteLine(pilot.Item1.Location + " - " + pilot.Item2.CurrentSpeed);
					}
				}


				Entity.VisitAll(thingModelVisitor);
				thingModelClient.Send();
			};
	        
			timer.Enabled = true;
			timer.Start();
			Thread.Sleep(Timeout.Infinite);
		}

		private void GenerateVictims(object sender, WharehouseEvents.ThingEventArgs thingEventArgs)
		{
			var thing = thingEventArgs.Thing;

			if (thing.Type == null)
			{
				return;
			}

			if (!thing.Type.Name.Contains(":incident:"))
			{
				return;	
			}

			var incidentLocation = thing.GetProperty<Property.Location>("location");

			if (incidentLocation == null)
			{
				return;
			}
			Console.WriteLine("VICTIMS");

			Oracle.Center = new Location(incidentLocation.Value.X, incidentLocation.Value.Y);

			var nbVictims = Convert.ToInt32(Math.Max(2, Oracle.CreateNumber(20, 4)));

			for (var i = 0; i < nbVictims; ++i)
			{
				
                var p = new Patient
                    {
                        Location = Oracle.CreateLocation(Oracle.AreaRadius)
                    };

				// MOUAHHAHAHHA
                p.Hit();
                p.AutomaticTriage();
                
				var motion = new Motion(1);

                _victims.Add(new Tuple<Patient, Motion>(p, motion));
			}
		}
		
		private void ManageRouting(object sender, WharehouseEvents.ThingEventArgs thingEventArgs)
		{
			var thing = thingEventArgs.Thing;

			if (thing.Type == null)
			{
				return;
			}

			if (!thing.Type.Name.Contains(":order"))
			{
				return;	
			}

			var location = thing.GetProperty<Property.Location>("location");
			if (location == null)
			{
				return;
			}

			var loc = new Location(location.Value.X, location.Value.Y);

			foreach (var connectedThing in thing.ConnectedThings)
			{
				Console.WriteLine(connectedThing.ID);
				var t = new ThingModelEntity(connectedThing);
				var path = new HereRoute(t.Location, loc);
				Console.WriteLine(t.Location);
				Console.WriteLine(loc);

				var pilot = new LocationPilot(t, path.Route, path.Speeds)
				{
					MaxSpeed = 300.5
				};
               
				_pilots.Add(new Tuple<ThingModelEntity, LocationPilot>(t, pilot));
			}
		}

	}
}
