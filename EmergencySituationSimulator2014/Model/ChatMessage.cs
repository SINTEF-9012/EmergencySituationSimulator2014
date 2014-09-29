using EmergencySituationSimulator2014.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmergencySituationSimulator2014.Model
{
    public class ChatMessage : Entity
    {
        public string Message { get; set; }
        

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
