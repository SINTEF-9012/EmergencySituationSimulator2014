using System;

namespace EmergencySituationSimulator2013
{
    class Oracle
    {
        public static Random Generator;
        public static Location Center;
        public static double AreaRadius;

        public static Location CreateLocation(Double areaRadius = 0, Location center = null)
        {
            if (center == null)
            {
                center = Center;
            }
            var location = new Location(center);

            if (Math.Abs(areaRadius - 0) < Double.Epsilon)
            {
                areaRadius = AreaRadius;
            }

            location.Move(Generator.NextDouble()*360.0,
                Generator.NextGaussianDouble(areaRadius/2.0, areaRadius/4.0));

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
        
        public static string CreatePersonName()
        {
            return Faker.Name.GetName();
        }
        
        public static double CreateAge()
        {
            // TODO improve it
            return Generator.NextDouble()*100;
        }
    }
}
