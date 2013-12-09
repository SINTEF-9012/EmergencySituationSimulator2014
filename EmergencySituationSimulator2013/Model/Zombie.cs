using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013.Model
{
    class Zombie : Patient
    {
        public Zombie() : base()
        {
            _pv /= 4; // Zombies are fragile
        }
        public override void Accept(Visitors.IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
