using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013.HereAPI
{
    class LocationType
    {
        public GeoCoordinateType DisplayPosition { get; set; }
        public List<AddressType> Address { get; set; }
    }
}
