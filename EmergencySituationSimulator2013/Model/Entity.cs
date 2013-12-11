using System;
using System.Collections.Generic;
using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013.Model
{
    public abstract class Entity
    {
        public static List<Entity> Instances = new List<Entity>();

        protected Guid Guid;
        private readonly string _id;

        public string Id { get { return _id; } }

        public Location Location { get; private set; }

        public string Name { get; protected set; }

        protected Entity()
        {
            Guid = Guid.NewGuid();
            _id = Guid.ToString("D");

            Location = new Location();

            Instances.Add(this);
        }

        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public static void VisitAll(IVisitor visitor)
        {
            foreach (var instance in Instances)
            {
                instance.Accept(visitor);
            }
        }


    }
}
