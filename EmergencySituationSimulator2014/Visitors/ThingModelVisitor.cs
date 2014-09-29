using System;
using EmergencySituationSimulator2014.Model;
using ThingModel;
using ThingModel.Builders;

namespace EmergencySituationSimulator2014.Visitors
{
	class ThingModelVisitor : IVisitor
	{
		private Warehouse _warehouse;
		private ThingType _typeEntity;
		private ThingType _typeWheeledVehicle;
		private ThingType _typePatient;
		private ThingType _typeZombie;
        private ThingType _typeIncident;
        private ThingType _typeResourceAllocation;
        private ThingType _typeChatMessage;

		public ThingModelVisitor(Warehouse warehouse)
		{
			_warehouse = warehouse;

			_typeEntity = BuildANewThingType.Named("ESS14:entity")
				.WhichIs("EmergencySituationSimulator root thing")
				.ContainingA.String("name")
				.AndA.Location("location");

			_typePatient = BuildANewThingType.Named("ESS14:person:patient")
				.WhichIs("EmergencySituationSimulator patient")
				.ContainingA.CopyOf(_typeEntity)
				.AndA.Double("healt")
				.AndAn.Int("age")
				.AndA.String("sex")
				.AndA.String("triage_status", "Triage status");

			_typeZombie = BuildANewThingType.Named("ESS14:person:zombie")
				.ContainingA.CopyOf(_typeEntity);

			_typeWheeledVehicle = BuildANewThingType.Named("ESS14:vehicle:wheeled")
				.WhichIs("EmergencySituationSimulator wheeled vehicle")
				.AndA.CopyOf(_typeEntity);

            _typeIncident = BuildANewThingType.Named("incidentType")
                .WhichIs("incident")
                .AndA.CopyOf(_typeEntity)
                .ContainingA.String("name")
                .AndA.NotRequired.String("description");

            _typeChatMessage = BuildANewThingType.Named("ChatMessageType")
                .WhichIs("ChatMessage")
                .ContainingA.String("author")
		        .AndA.String("content") 
		        .AndA.DateTime("datetime")
		        .AndA.NotRequired.Int("number");

            _typeResourceAllocation = BuildANewThingType.Named("master:order")
		.WhichIs("Order")
		.ContainingA.Location("location")
		.AndA.String("task")
		.AndA.NotRequired.String("description")
		.AndA.Boolean("accepted")
		.AndA.Boolean("done");

			/*_warehouse.RegisterType(_typeEntity);
			_warehouse.RegisterType(_typeWheeledVehicle);
			_warehouse.RegisterType(_typePatient);
			_warehouse.RegisterType(_typeZombie);
            _warehouse.RegisterType(_typeIncident);
            _warehouse.RegisterType(_typeResourceAllocation);
            _warehouse.RegisterType(_typeChatMessage);*/
		}

		public BuildANewThing.ThingPropertyBuilder GenerateThing(Entity e, ThingType type)
		{
			var loc = e.Location;

			return BuildANewThing.As(type)
				.IdentifiedBy(e.Id)
				.ContainingA.Location("location", new ThingModel.Location.LatLng(
					Math.Round(loc.lat, 7), Math.Round(loc.lng,7)))
				.AndA.String("name", e.Name);
		}

		public void Register(Thing thing)
		{
			_warehouse.RegisterThing(thing);	
		}

		public void Visit(Entity e)
		{
			Register(GenerateThing(e, _typeEntity));
		}

		public void Visit(WheeledVehicle v)
		{
			var vehicle = GenerateThing(v, _typeWheeledVehicle);

			Register(vehicle);
		}

        public void Visit(Incident i)
        {
            var incident = GenerateThing(i, _typeIncident);
            incident.ContainingA.String("name", i.Name)
                .AndA.String("description", i.Description);            
            Register(incident);
        }

        public void Visit(ResourceAllocation r)
        {
            var resourceAllocation = BuildANewThing.As(_typeResourceAllocation)
                .IdentifiedBy(r.Id)
            .ContainingA.String("task", r.Task)
                .AndA.String("resourceID", r.ResourceMobilizationID)
                .AndA.Boolean("accepted", false)
                .AndA.Boolean("done", false);            
            Register(resourceAllocation);
        }

        public void Visit(ChatMessage c)
        {
            var chatMessage = BuildANewThing.As(_typeChatMessage)
				.IdentifiedBy(c.Id)
            .ContainingA.String("content", c.Message)
                .AndA.String("author", c.Id)
                .AndA.DateTime("datetime", DateTime.UtcNow);
            Register(chatMessage);
        }

		public void Visit(Patient v)
		{
			var patient = GenerateThing(v, _typePatient);
			patient.ContainingA.Double("healt", v.HealtStatus)
				.AndAn.Int("age", (int) v.Age);

			switch (v.Sex)
			{
				case Person.SexEnum.Woman:
					patient.ContainingA.String("sex", "woman");
					break;
				case Person.SexEnum.Man:
					patient.ContainingA.String("sex", "man");
					break;
				default:
					patient.ContainingA.String("sex", "other");
					break;
			}

			switch (v.Triage)
			{
				case Patient.TriageEnum.Black:
					patient.ContainingA.String("triage_status", "BLACK");
					break;
				case Patient.TriageEnum.Red:
					patient.ContainingA.String("triage_status", "RED");
					break;
				case Patient.TriageEnum.Yellow:
					patient.ContainingA.String("triage_status", "YELLOW");
					break;
				case Patient.TriageEnum.Green:
					patient.ContainingA.String("triage_status", "GREEN");
					break;
				case Patient.TriageEnum.White:
					patient.ContainingA.String("triage_status", "WHITE");
					break;
			}

			Register(patient);
		}

		public void Visit(Zombie z)
		{
			Register(GenerateThing(z, _typeZombie));
		}

		public void Visit(ThingModelEntity e)
		{
			// Just update the location
			_warehouse.NotifyThingUpdate(e.Thing,null);
		}
	}
}
