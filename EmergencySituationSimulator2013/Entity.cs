using System;

namespace EmergencySituationSimulator2013
{
    public class Entity
    {
        public Location location { get; private set; }

        public Guid ID { get; private set; }
        public string sID { get; private set; }

        public Motion motion { get; private set; }

        public object content;

        public Entity(double speedRatio = 1.0)
        {
            ID = Guid.NewGuid();
            sID = ID.ToString("D");

            location = new Location();
            motion = new Motion(speedRatio);
        }

        /*public delegate void CommitEventHandler(Entity entity);

        public event CommitEventHandler CommitEvent;

        public void Commit()
        {
            if (CommitEvent != null)
            {
                CommitEvent(this);
            }
            // TODO thing on content (like update the position)
            //new L.Marker([0,0]).addTo(map)
            //Console.WriteLine(sID.Substring(0,10) + ": " + location);
            Console.WriteLine("new L.Marker([" + location.ToString().Replace(",",".").Replace(" ", ", ")+"]).addTo(map);");
        }*/

        public void Move(Random random, double time)
        {
            var movement = motion.Move(random, time);
            location.Move(movement.Item1, movement.Item2);
        }
    }
}