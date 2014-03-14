using System;
using EmergencySituationSimulator2014.Visitors;
using ThingModel;

namespace EmergencySituationSimulator2014.Model
{
	class ThingModelEntity:Entity
	{
		private readonly Thing _thing;
		public Thing Thing
		{
			get
			{
				return _thing;
			}
		}

		public ThingModelEntity(Thing thing) : base(thing.ID)
		{
			_thing = thing;
			var location = _thing.GetProperty<Property.Location>("location");

			if (location != null)
			{
				Location.lat = location.Value.X;
				Location.lng = location.Value.Y;
			}
		}

		private void UpdateLocation()
		{
			var location = _thing.GetProperty<Property.Location>("location");

			if (location != null)
			{
				if (!Double.IsNaN(Location.lat) && !Double.IsNaN(Location.lng))
				{
					location.Value.X = Location.lat;
					location.Value.Y = Location.lng;
				}
			}
		}

		public override void Accept(IVisitor visitor)
		{
			UpdateLocation();
            visitor.Visit(this);
		}
	}
}
