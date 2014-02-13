using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013.Model
{
    class PoliceCar : WheeledVehicle
    {
        public PoliceCar()
        {
            
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
