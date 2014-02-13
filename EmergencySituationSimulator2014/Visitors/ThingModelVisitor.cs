using EmergencySituationSimulator2014.Model;
using ThingModel;

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

			_typeEntity = new ThingType("EmergencySituationSimulator2014:entity");
			_typeEntity.Description = "EmergencySituationSimulator root thing";
			_typeEntity.DefineProperty(PropertyType.Create<Property.String>("name"));
			_typeEntity.DefineProperty(PropertyType.Create<Property.Location>("location"));
			
			_typePatient = new ThingType("EmergencySituationSimulator2014:person:patient");
			_typePatient.Description = "EmergencySituationSimulator patient";
			_typePatient.DefineProperty(PropertyType.Create<Property.String>("name"));
			_typePatient.DefineProperty(PropertyType.Create<Property.Location>("location"));
			_typePatient.DefineProperty(PropertyType.Create<Property.Double>("healt"));
			_typePatient.DefineProperty(PropertyType.Create<Property.Int>("age"));
			_typePatient.DefineProperty(PropertyType.Create<Property.String>("sex"));
			_typePatient.DefineProperty(PropertyType.Create<Property.String>("triage_status"));

			_typeZombie = new ThingType("EmergencySituationSimulator2014:person:zombie");
			_typeZombie.Description = "A Zombie for a very realistic situation";
			_typeZombie.DefineProperty(PropertyType.Create<Property.String>("name"));
			_typeZombie.DefineProperty(PropertyType.Create<Property.Location>("location"));

			_typeWheeledVehicle = new ThingType("EmergencySituationSimulator2014:vehicle:wheeled");
			_typeWheeledVehicle.Description = "EmergencySituationSimulator wheeled vehicle";
			_typeWheeledVehicle.DefineProperty(PropertyType.Create<Property.String>("name"));
			_typeWheeledVehicle.DefineProperty(PropertyType.Create<Property.Location>("location"));

			_wharehouse.RegisterType(_typeEntity);
			_wharehouse.RegisterType(_typeWheeledVehicle);
			_wharehouse.RegisterType(_typePatient);
			_wharehouse.RegisterType(_typeZombie);
		}

		public Thing GenerateThing(Entity e, ThingType type)
		{
			var thing = new Thing(e.Id, type);

			var loc = e.Location;

			thing.SetProperty(new Property.Location("location",
				new ThingModel.Location.LatLng(loc.lat, loc.lng)));

			thing.SetProperty(new Property.String("name", e.Name));

			return thing;
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
			patient.SetProperty(new Property.Double("healt", v.HealtStatus));

			patient.SetProperty(new Property.Int("age", (int) v.Age));

			switch (v.Sex)
			{
				case Person.SexEnum.Woman:
					patient.SetProperty(new Property.String("sex", "woman"));
					break;
				case Person.SexEnum.Man:
					patient.SetProperty(new Property.String("sex", "man"));
					break;
				default:
					patient.SetProperty(new Property.String("sex", "other"));
					break;
			}

			switch (v.Triage)
			{
				case Patient.TriageEnum.Black:
					patient.SetProperty(new Property.String("triage_status", "BLACK"));
					break;
				case Patient.TriageEnum.Red:
					patient.SetProperty(new Property.String("triage_status", "RED"));
					break;
				case Patient.TriageEnum.Yellow:
					patient.SetProperty(new Property.String("triage_status", "YELLOW"));
					break;
				case Patient.TriageEnum.Green:
					patient.SetProperty(new Property.String("triage_status", "GREEN"));
					break;
				case Patient.TriageEnum.White:
					patient.SetProperty(new Property.String("triage_status", "WHITE"));
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
