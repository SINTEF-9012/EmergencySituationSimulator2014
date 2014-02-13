using EmergencySituationSimulator2014.Visitors;

namespace EmergencySituationSimulator2014.Model
{
    class PoliceMan : RescuePerson
    {
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
