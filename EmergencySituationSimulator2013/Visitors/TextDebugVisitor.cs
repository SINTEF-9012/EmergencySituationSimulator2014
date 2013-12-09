using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013.Visitors
{
    class TextDebugVisitor : IVisitor
    {
        private StringBuilder _sb = new StringBuilder();

        private void AppendLocation(Entity e)
        {
            _sb.Append(e.Location.ToString());
        }

        public void Visit(Entity e)
        {
            Console.WriteLine("canard");
        }

        public void Visit(WheeledVehicle v)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WheeledVehicule: ");
            sb.Append(v.Id);
            sb.Append(" ");
            AppendLocation(v);
            Console.WriteLine(sb);
            sb.Clear();
        }
    }
}
