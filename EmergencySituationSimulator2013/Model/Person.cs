using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013.Model
{
    public abstract class Person : Entity
    {
        public double Pv { get; protected set; }
        public double GoodHealtPv { get; protected set; }

        public double Age { get; protected set; }

        public double HealtStatus
        {
            get { return Pv/GoodHealtPv; }
        }

        public enum SexEnum
        {
            Woman,
            Man,
            Other
        }

        public SexEnum Sex { get; protected set; }

        protected Person()
        {
            GoodHealtPv = 100.0;

            Pv = Math.Max(Oracle.CreateNumber(100.0, 42.0), 10.0);

            Name = Oracle.CreatePersonName();

            // Maybe I should rename the isFortunate method :P
            Sex = Oracle.IsFortunate() ? SexEnum.Woman : SexEnum.Man;

            Age = Oracle.CreateAge();

        }
        
        public void Hit()
        {
            Pv += Math.Min(Oracle.CreateNumber(-30.0, 30.0), 0);
        }
    }
}
