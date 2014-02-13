using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2014.Model
{
    public class Zombie : Patient
    {
        public Zombie() : base()
        {
            Pv /= 4; // Zombies are fragile
        }
        public override void Accept(Visitors.IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
