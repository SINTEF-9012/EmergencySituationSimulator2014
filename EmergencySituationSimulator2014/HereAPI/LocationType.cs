using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2014.HereAPI
{
    class LocationType
    {
        public GeoCoordinateType DisplayPosition { get; set; }
        public List<AddressType> Address { get; set; }
    }
}
