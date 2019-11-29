namespace Cornichon.Tests.CommerceExample.FakeCommerce
{
    public class Receipt
    {
        public Receipt(Order order) => Order = order;

        public Order Order { get; }
    }
}
