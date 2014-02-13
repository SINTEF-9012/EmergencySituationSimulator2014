using System;

namespace EmergencySituationSimulator2013
{
    public class Random : System.Random
    {
        public Random()
        {
        }

        public Random(int seed) : base(seed)
        {
        }

        public static Random fromSeed(string seed)
        {
            if (seed != "random")
            {
                // Naive seed converter
                int iSeed = 0, cpt = 0;
                foreach (char c in seed)
                {
                    iSeed = c*(c + (iSeed >> ++cpt));
                }
                return new Random(iSeed);
            }
            return new Random();
        }

        public double NextGaussianDouble(double stdDev = 1.0, double mean = 0.0)
        {
            // http://stackoverflow.com/questions/218060/random-gaussian-variables
            double u1 = NextDouble(); //these are uniform(0,1) random doubles
            double u2 = NextDouble();
            double randStdNormal = Math.Sqrt(-2.0*Math.Log(u1))*
                                   Math.Sin(2.0*Math.PI*u2); //random normal(0,1)
            double randNormal =
                mean + stdDev*randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}