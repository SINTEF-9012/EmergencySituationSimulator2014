using EmergencySituationSimulator2014.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmergencySituationSimulator2014.Model
{
    public class ResourceAllocation : Entity
    {
        public string ResourceMobilizationID { get; set; }
        public int NumberOfFireVehicles { get; set; }
        public int NumberOfFirePersonnel { get; set; }
        public int NumberOfMedicVehicles { get; set; }
        public int NumberOfMedicPersonnel { get; set; }
        public int NumberOfPoliceVehicles { get; set; }
        public int NumberOfPolicePersonnel { get; set; }
        public string Task { get; set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
