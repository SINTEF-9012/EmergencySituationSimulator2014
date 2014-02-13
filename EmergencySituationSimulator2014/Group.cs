using System.Collections.Generic;
using EmergencySituationSimulator2014.Model;

namespace EmergencySituationSimulator2014
{
    class Group
    {
        public List<Entity> Entities { get; private set; }

        public Motion Motion { get; private set; }

        public Group(double speedRatio = 1.0)
        {
            Entities = new List<Entity>();
            Motion = new Motion(speedRatio);
        }

        public void Move(Random random, double time)
        {
            var movment = Motion.Move(random, time);
            foreach (var entity in Entities)
            {
                entity.Location.Move(movment.Item1,movment.Item2);
            }
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);
        }
    }
}
