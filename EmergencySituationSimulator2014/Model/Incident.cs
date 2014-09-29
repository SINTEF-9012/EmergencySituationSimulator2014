using EmergencySituationSimulator2014.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmergencySituationSimulator2014.Model
{
    public class Incident : Entity 
    {
        public string Description { get; set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
