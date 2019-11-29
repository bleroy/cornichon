using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cornichon.Tests
{
    public class CornichonTests
    {
        [Fact]
        public async void CanAwaitAsyncScenario()
        {
            var sequence = new List<int> { 0 };
            await Scenario.Given(async () =>
            {
                sequence.Add(1);
                await Task.Delay(200);
                sequence.Add(2);
            });
            sequence.Add(3);
            Assert.Equal(new[] { 0, 1, 2, 3 }, sequence);
        }

        [Fact]
        public void CanRunScenarioSequentiallyAndSynchronously()
        {
            var sequence = new List<int> { 0 };
            Scenario
                .Given(() => sequence.Add(1))
                  .And(() => sequence.Add(2))
                 .When(() => sequence.Add(3))
                 .Then(() => sequence.Add(4));
            sequence.Add(5);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, sequence);
        }

        [Fact]
        public async void CanRunMixedScenarioOfAsyncAndSync()
        {
            var sequence = new List<int> { 0 };
            await Scenario
                .Given(() => sequence.Add(1))
                .And(async () =>
                {
                    await Task.Delay(200);
                    sequence.Add(2);
                })
                .When(() => sequence.Add(3))
                .Then(async () =>
                {
                    await Task.Delay(200);
                    sequence.Add(4);
                });
            sequence.Add(5);
            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, sequence);
        }
    }
}
