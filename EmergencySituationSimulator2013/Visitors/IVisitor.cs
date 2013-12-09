using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013.Visitors
{
    interface IVisitor
    {
        void Visit(Entity e);
        void Visit(WheeledVehicle v);
    }
}
