using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013.Visitors
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
    }
}
