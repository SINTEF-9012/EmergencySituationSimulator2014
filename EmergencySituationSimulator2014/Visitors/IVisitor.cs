using EmergencySituationSimulator2014.Model;

namespace EmergencySituationSimulator2014.Visitors
{
    public interface IVisitor
    {
        void Visit(Entity e);
        void Visit(WheeledVehicle v);
        void Visit(Patient v);
        void Visit(Zombie z);
    }
}
