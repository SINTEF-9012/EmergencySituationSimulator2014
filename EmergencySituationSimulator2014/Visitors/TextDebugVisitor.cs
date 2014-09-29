using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencySituationSimulator2014.Model;

namespace EmergencySituationSimulator2014.Visitors
{
    class TextDebugVisitor : IVisitor
    {
        private readonly StringBuilder Sb = new StringBuilder();

        private void AppendLocation(Entity e)
        {
            Sb.Append(e.Location.ToString());
        }

        private void Print()
        {
            Console.WriteLine(Sb);
            Sb.Clear();
        }

        public void Visit(Entity e)
        {
            Console.WriteLine("canard");
        }

        public void Visit(WheeledVehicle v)
        {
            Sb.Append("WheeledVehicule: ");
            Sb.Append(v.Id);
            Sb.Append(" ");
            AppendLocation(v);
            Print();
        }

        public void Visit(Patient v)
        {
            Sb.Append("Victim: ");
            Sb.Append(v.Name);
            Print();
        }

        public void Visit(Zombie z)
        {
            Sb.Append("Zombie: ");
            Sb.Append(z.Name);
            Print();
        }

	    public void Visit(ThingModelEntity e)
	    {
            Sb.Append("Thing: ");
            Sb.Append(e.Name);
            Print();
	    }

        public void Visit(Incident i)
        {
            Sb.Append("Incident: ");
            Sb.Append(i.Name);
            Print();
        }

        public void Visit(ResourceAllocation r)
        {
            Sb.Append("ResourceAllocation: ");
            Sb.Append(r.Name);
            Print();
        }

        public void Visit(ChatMessage c)
        {
            Sb.Append("ChatMessage: ");
            Sb.Append(c.Name);
            Print();
        }
    }
}
