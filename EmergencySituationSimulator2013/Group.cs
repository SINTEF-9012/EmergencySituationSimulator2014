using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013
{
    class Group
    {
        public List<Entity> entities { get; private set; }

        public Motion motion { get; private set; }

        public Group(double speedRatio = 1.0)
        {
            entities = new List<Entity>();
            motion = new Motion(speedRatio);
        }

        public void Move(Random random, double time)
        {
            var movment = motion.Move(random, time);
            foreach (var entity in entities)
            {
                entity.location.Move(movment.Item1,movment.Item2);
            }
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }
    }
}
