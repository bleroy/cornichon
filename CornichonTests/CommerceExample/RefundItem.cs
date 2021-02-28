using Cornichon.Tests.CommerceExample.FakeCommerce;
using System.Threading.Tasks;
using Xunit;

namespace Cornichon.Tests.CommerceExample
{
    /// <summary>
    /// Refund item.
    /// </summary>
    public class RefundItem
    {
        [Fact]
        public void CustomerReturningProductsCausesRefund()
        {
            var jeff = new Customer
            {
                Balance = 0
            };
            var order = new Order(new Product("microwave", 100));

            Scenario
                .Given(() => { jeff.Orders.Add(order); })
                  .And(() => { jeff.Receipts.Add(order.Receipt); })
                 .When(() => { jeff.Return(order.Products[0]); })
                 .Then(() => { Assert.Equal(100, jeff.Balance); });
        }

        private readonly CustomerTestHelper jeff = new CustomerTestHelper();

        [Fact]
        public void PrettierCustomerReturningProductsCausesRefund()
            => Scenario
                .Given(() => { jeff.bought_a("microwave").at(100) ; })
                  .And(() => { jeff.has_a_receipt(); })
                 .When(() => { jeff.returns_the("microwave"); })
                 .Then(() => { jeff.should_have(100); });

        [Fact]
        public async ValueTask AsynchronousCustomerReturningProductsCausesRefund()
            => await Scenario
                .Given(async () => { await jeff.bought_a("microwave").asynchronously_for(100); })
                  .And(      () => {       jeff.has_a_receipt(); })
                 .When(async () => { await jeff.asynchronously_returns_the("microwave"); })
                 .Then(      () => {       jeff.should_have(100); });
    }
}
