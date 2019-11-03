using System;

namespace Cornichon
{
    public class Scenario
    {
        private Scenario()
        { }

        public static Scenario Given(Action axiom)
        {
            axiom.Invoke();
            return new Scenario();
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
