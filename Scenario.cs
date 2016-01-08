using System;

namespace Cornichon
{
    public class Scenario
    {
        public Scenario(string description)
        {
            Description = description;
        }

        public string Description { get; set; }

        public Scenario Given(Action axiom)
        {
            axiom.Invoke();
            return this;
        }

        public Scenario And(Action additionalStepOrAssertion)
        {
            additionalStepOrAssertion.Invoke();
            return this;
        }

        public Scenario When(Action step)
        {
            step.Invoke();
            return this;
        }

        public Scenario Then(Action assertion)
        {
            assertion.Invoke();
            return this;
        }
    }
}
