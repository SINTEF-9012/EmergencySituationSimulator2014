using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013.Model
{
    class PoliceMan : RescuePerson
    {
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
