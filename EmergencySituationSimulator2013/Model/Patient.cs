using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013.Model
{
    public class Patient : Person
    {
        public enum TriageEnum
        {
           Unknown, Black, Red, Yellow, Green, White
        }

        public TriageEnum Triage;

        public void AutomaticTriage()
        {
            var healtStatus = HealtStatus;

            if (healtStatus > 0.9)
            {
                // Just minor injuries
                Triage = TriageEnum.White;
            }
            if (healtStatus > 0.66)
            {
                // Require a doctor's care but not immediately
                Triage = TriageEnum.Green;
            }
            else if (healtStatus > 0.33)
            {
                // Condition stable but require watching
                Triage = TriageEnum.Yellow;
            }
            else if (healtStatus > 0.1)
            {
                // Priority
                Triage = TriageEnum.Red;
            }
            else
            {
                // It's sad
                Triage = TriageEnum.Black;
            }
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
