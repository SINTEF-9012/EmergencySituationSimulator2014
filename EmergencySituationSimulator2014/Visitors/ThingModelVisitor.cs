using EmergencySituationSimulator2014.Model;
using ThingModel;
using ThingModel.Builders;

namespace EmergencySituationSimulator2014.Visitors
{
	class ThingModelVisitor : IVisitor
	{
		private Wharehouse _wharehouse;
		private ThingType _typeEntity;
		private ThingType _typeWheeledVehicle;
		private ThingType _typePatient;
		private ThingType _typeZombie;

		public ThingModelVisitor(Wharehouse wharehouse)
		{
			_wharehouse = wharehouse;

			_typeEntity = BuildANewThingType.Named("ESS14:entity")
				.WichIs("EmergencySituationSimulator root thing")
				.ContainingA.String("name")
				.AndA.Location("location");

			_typePatient = BuildANewThingType.Named("ESS14:person:patient")
				.WichIs("EmergencySituationSimulator patient")
				.ContainingA.CopyOfThis(_typeEntity)
				.AndA.Double("healt")
				.AndAn.Int("age")
				.AndA.String("sex")
				.AndA.String("triage_status", "Triage status");

			_typeZombie = BuildANewThingType.Named("ESS14:person:zombie")
				.ContainingA.CopyOfThis(_typeEntity);

			_typeWheeledVehicle = BuildANewThingType.Named("ESS14:vehicle:wheeled")
				.WichIs("EmergencySituationSimulator wheeled vehicle")
				.AndA.CopyOfThis(_typeEntity);

			_wharehouse.RegisterType(_typeEntity);
			_wharehouse.RegisterType(_typeWheeledVehicle);
			_wharehouse.RegisterType(_typePatient);
			_wharehouse.RegisterType(_typeZombie);
		}

		public BuildANewThing.ThingPropertyBuilder GenerateThing(Entity e, ThingType type)
		{
			var loc = e.Location;

			return BuildANewThing.As(type)
				.IdentifiedBy(e.Id)
				.ContainingA.Location("location", new ThingModel.Location.LatLng(loc.lat, loc.lng))
				.AndA.String("name", e.Name);
		}

		public void Register(Thing thing)
		{
			_wharehouse.RegisterThing(thing);	
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
	}
}
