using System;

namespace EmergencySituationSimulator2014
{
    public class Motion
    {
        public double direction;
        public double speed;
        public MotionState State = MotionState.RANDOM;

        // Usefull for set the speed of an entities's class
        private double speedRatio;

        public Motion(double speedRatio)
        {
            this.speedRatio = speedRatio;
        }

        // This is callibrated for 50fps
        public Tuple<double, double> Move(Random random, double time)
        {
            // Change direction ?
            if (random.Next(12) > 10)
            {
                direction = Math.Abs(random.NextGaussianDouble(180.0));
            }
            else
            {
                direction += random.NextGaussianDouble(10.0);
                if (direction < 0) direction = -direction;
            }

            // Change speed ?
            if (random.Next(12) > 10)
            {
                speed = Math.Abs(random.NextGaussianDouble(2.0, 2.0)) * speedRatio;
            }
            else
            {
                speed += random.NextGaussianDouble(0.5);
                if (speed < 0) speed = -speed;
            }

            var distance = speed*time; // 60fps

            return new Tuple<double, double>(direction, distance);
        }
    }
}