using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cornichon
{
    public class Scenario : INotifyCompletion
    {
        private Task _task;

        private Scenario() {}

        private Scenario(Func<Task> taskFactory)
        {
            _task = taskFactory();
        }

        public Exception Exception { get; private set; }

        public Scenario GetAwaiter() => this;

        public bool IsCompleted => _task?.IsCompleted ?? true;

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (_task is null)
            {
                continuation();
            }
            else
            {
                _task.ContinueWith(antecedent => continuation());
            }
        }

        public Scenario GetResult()
        {
            _task?.Wait();
            return this;
        }

        public static Scenario Given(Action axiom) => new Scenario().Then(axiom);

        public static Scenario Given(Func<Task> axiomTask) => new Scenario().Then(axiomTask);

        public Scenario And(Action additionalStepOrAssertion) => Then(additionalStepOrAssertion);

        public Scenario And(Func<Task> additionalStepOrAssertionTask) => Then(additionalStepOrAssertionTask);

        public Scenario When(Action step) => Then(step);

        public Scenario When(Func<Task> step) => Then(step);

        public Scenario Then(Action assertion)
        {
            _task?.Wait();
            assertion();
            return new Scenario();
        }

        public Scenario Then(Func<Task> assertionTask)
        {
            _task?.Wait();
            return new Scenario(assertionTask);
        }
    }
}
