using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013.Model
{
    abstract class Person : Entity
    {
        protected double _pv = 100;
        public double Pv { get { return _pv; } }

        public string Name { get; protected set; }

        protected Person()
        {
            _pv = Math.Max(Oracle.CreateNumber(100.0, 42.0), 10.0);

            Name = Oracle.CreateName();
        }
        
        public void Hit()
        {
            
        }
    }
}
