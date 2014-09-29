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
	public class ProgramReborn
	{
		private Warehouse _warehouse;
		private List<Tuple<Patient, Motion>> _victims;
		private List<Tuple<ThingModelEntity, LocationPilot>> _pilots;
        private ThingModel.WebSockets.Client _thingModelClient;
        private ThingModelVisitor _thingModelVisitor;
        private string[] _incidentObjectTypes;
        private Random _randomNumberGenerator;
        private LinksmartHandler _linksmartHandler;
        private HereGeocoder _hereGeoCoder;

        public ProgramReborn(string hereAppId, string hereAppCode, string seed, string areaRadius, string senderID, string connection)
        {
            Setup(hereAppId, hereAppCode, seed, areaRadius, senderID, connection);
        }

        private void Setup(string hereAppId, string hereAppCode, string seed, string areaRadius, string senderID, string connection)
		{
            // Load settings
            var appSettings = ConfigurationManager.AppSettings;

            // Setting up the Here API
            HereConfig.AppId = hereAppId;
            HereConfig.AppCode = hereAppCode;
            
			// Setting up the Oracle
            Oracle.Generator = Random.fromSeed(seed);
            double.TryParse(areaRadius, out Oracle.AreaRadius);

            // Setting up ThingModel connection
			_warehouse = new Warehouse();	        
			_thingModelClient = new Client(senderID, connection, _warehouse);
            _thingModelVisitor = new ThingModelVisitor(_warehouse);	
			
		    // Set incident object types
            _incidentObjectTypes = new string[38] { "Explosion Incident", "Fire Incident", "Rock Slide Incident", "Chemical Incident", "Bomb Incident", "Automobile Incident", "Perpetrator Incident", "Incident", "Control point", "Exit Point", "Entry Point", "Road Block", "Triage Point", "Ambulance standby area", "Evacuation Point", "Assembly area", "Assembly area dead", "Assembly area injured", "Assembly area evacuated", "Helicopter landing area", "Depot", "Alignment point", "Command post", "Meeting point", "Fire personnel meeting point", "Health personnel meeting point", "Police personnel meeting point", "Alignment point fire vehicles", "Alignment point police vehicles", "Alignment point health vehicles", "Response object", "Automobile Risk", "Bomb Risk", "Chemical Risk", "Explosion Risk", "Fire Risk", "Rock Slide Risk", "Risk" };

            // Set up randomizer
            _randomNumberGenerator = new Random();

            _hereGeoCoder = new HereGeocoder(seed);
		}

        public void StartIncidentSimulation(string seedOrigin, string seed, string amountOfObjects, string areaRadius, bool publishViaThingmodel)
        {
            // Set area
            var placeInfo = new HereGeocoder(seedOrigin);
            var location = placeInfo.Location;
            Oracle.Generator = Random.fromSeed(seed);
            Oracle.Center = location;
            double.TryParse(areaRadius, out Oracle.AreaRadius);

            // Set number of elements
            var numberOfElements = Convert.ToInt32(amountOfObjects);

            // Generate incidents
            var incidents = new List<Incident>();

            for (var i = 0; i < numberOfElements; ++i)
            {
                var incident = new Incident
                {
                    Location = Oracle.CreateLocation(Oracle.AreaRadius),
                    Name = GetRandomIncidentType(),
                    Description = LoremIpsum(5, 10, 1, 3, 1)
                };

                incidents.Add(incident);
            }

            if (publishViaThingmodel)
            {
                // Visit and send to thingmodel
                Entity.VisitAll(_thingModelVisitor);
                _thingModelClient.Send();
            }
            else
            {
                if (_linksmartHandler == null)
                {
                    _linksmartHandler = new LinksmartHandler();
                }

                if (_linksmartHandler.IsConnectedToLinksmart)
                {
                    foreach (var i in incidents) {
                        _linksmartHandler.SendIncidentObjectToS2D2S(i);
                    }
                }
            }
            
        }

        public void StartResourceAllocationSimulation(string seedOrigin, string seed, string amountOfObjects, string areaRadius, bool publishViaThingmodel)
        {
            // Set area
            var placeInfo = new HereGeocoder(seedOrigin);
            var location = placeInfo.Location;
            Oracle.Generator = Random.fromSeed(seed);
            Oracle.Center = location;
            double.TryParse(areaRadius, out Oracle.AreaRadius);

            // Set number of elements
            var numberOfElements = Convert.ToInt64(amountOfObjects);           

            // Get resource IDs
            var things = _warehouse.Things;

            var resourceIDs = new List<string>();

            foreach (Thing thing in things)
            {
                if (thing.Type != null && thing.Type.Name.Equals("ResourceType"))
                {
                    resourceIDs.Add(thing.ID);
                }
            }

            // Generate resource allocations
            var resourceAllocations = new List<ResourceAllocation>();

            if (resourceIDs.Count < numberOfElements)
            {
                numberOfElements = resourceIDs.Count;
            }
            
            for (var i = 0; i < numberOfElements; ++i)
            {
                var resourceAllocation = new ResourceAllocation
                {
                    Location = Oracle.CreateLocation(Oracle.AreaRadius),
                    Task = LoremIpsum(3, 10, 1, 3, 1),
                    ResourceMobilizationID = resourceIDs[i],
                    
                };

                resourceAllocations.Add(resourceAllocation);
            }

            if (publishViaThingmodel)
            {
                // Visit and send to thingmodel
                Entity.VisitAll(_thingModelVisitor);
                _thingModelClient.Send();
            }
            else
            {
                if (_linksmartHandler == null)
                {
                    _linksmartHandler = new LinksmartHandler();
                }

                if (_linksmartHandler.IsConnectedToLinksmart)
                {
                    foreach (var r in resourceAllocations)
                    {
                        _linksmartHandler.SendResourceAllocationToS2D2S(r);
                    }
                }
            }
        }

        public void StartChatMessagesSimulation(string amountOfObjects, bool publishViaThingmodel)
        {
            // Set number of elements
            var numberOfElements = Convert.ToInt64(amountOfObjects);

            // Generate chat messages
            var chatMessages = new List<ChatMessage>();

            for (var i = 0; i < numberOfElements; ++i)
            {
                var chatMessage = new ChatMessage
                {
                    //Location = Oracle.CreateLocation(Oracle.AreaRadius),
                    Message = "Test message",
                    
                };

                chatMessages.Add(chatMessage);
            }

            if (publishViaThingmodel)
            {
                // Visit and send to thingmodel
                Entity.VisitAll(_thingModelVisitor);
                _thingModelClient.Send();
            }
            else
            {
                if (_linksmartHandler == null)
                {
                    _linksmartHandler = new LinksmartHandler();
                }

                if (_linksmartHandler.IsConnectedToLinksmart)
                {
                    foreach (var c in chatMessages)
                    {
                        _linksmartHandler.SendChatMessageToS2D2S(c);
                    }
                }
            }
        }        

        private string GetRandomIncidentType()
        {
            return _incidentObjectTypes[_randomNumberGenerator.Next(0,37)];
        }
		
		private void ManageRouting(object sender, WarehouseEvents.ThingEventArgs thingEventArgs)
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

        private string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            string result = string.Empty;

            for (int p = 0; p < numParagraphs; p++)
            {
                result += "<p>";
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result += " "; }
                        result += words[rand.Next(words.Length)];
                    }
                    result += ". ";
                }
                result += "</p>";
            }

            return result;
        }
        
    }
}
