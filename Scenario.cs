using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cornichon
{
    public class Scenario : INotifyCompletion
    {
        private Task _task;

        private Scenario() {}

        public Scenario GetAwaiter() => this;

        public bool IsCompleted => _task?.IsCompleted ?? true;

        void INotifyCompletion.OnCompleted(Action continuation) => Queue(continuation);

        public Scenario GetResult() => this;

        private void Queue(Action continuation)
        {
            if (_task is null)
            {
                continuation();
                return;
            }
            Queue(() => Task.Run(
                () => continuation()));
        }

        private void Queue(Func<Task> taskFactory)
        {
            if (_task is null)
            {
                _task = taskFactory();
                return;
            }
            // Capture the previous task in the closure
            Task antecedent = _task;
            _task = Task.Run(async () =>
            {
                await antecedent;
                await taskFactory();
            });
        }

        public static Scenario Given(Action axiom) => new Scenario().Then(axiom);

        public static Scenario Given(Func<Task> axiomTask) => new Scenario().Then(axiomTask);

        public Scenario And(Action additionalStepOrAssertion) => Then(additionalStepOrAssertion);

        public Scenario And(Func<Task> additionalStepOrAssertionTask) => Then(additionalStepOrAssertionTask);

        public Scenario When(Action step) => Then(step);

        public Scenario When(Func<Task> step) => Then(step);

        public Scenario Then(Action assertion)
        {
            ((INotifyCompletion)this).OnCompleted(assertion);
            return this;
        }

        public Scenario Then(Func<Task> assertionTask)
        {
            Queue(assertionTask);
            return this;
        }
    }
}
