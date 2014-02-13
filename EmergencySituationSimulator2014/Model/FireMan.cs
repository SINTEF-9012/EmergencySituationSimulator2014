using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencySituationSimulator2014.Visitors;

namespace EmergencySituationSimulator2014.Model
{
    class FireMan : RescuePerson
    {
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
