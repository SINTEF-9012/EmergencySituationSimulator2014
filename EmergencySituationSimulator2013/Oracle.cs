using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencySituationSimulator2013
{
    class Oracle
    {
        public static Random Generator;
        public static Location Center;
        public static int AreaRadius;

        public static Location CreateLocation()
        {
            var location = new Location(Center);

            location.Move(Generator.NextDouble()*360.0,
                Generator.NextGaussianDouble(AreaRadius/2, AreaRadius/4));

            return location;
        }

        public static bool IsFortunate(int nbChances = 1, int nbAttempts = 2)
        {
            return Generator.Next(nbAttempts) < nbChances;
        }

        public static double CreateNumber(double typicalValue = 0.0, double hazardous = 1.0)
        {
            return Generator.NextGaussianDouble(hazardous, typicalValue);
        }
        
        public static string CreateName()
        {
            return Faker.Name.GetName();
        }
    }
}
