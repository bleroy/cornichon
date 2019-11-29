using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cornichon.Tests.CommerceExample.FakeCommerce
{
    public class Customer
    {
        public decimal Balance { get; set; }
        public IList<Order> Orders { get; } = new List<Order>();
        public IList<Receipt> Receipts { get; } = new List<Receipt>();

        public void Return(Product product)
        {
            Balance += product.Price;
        }

        public async Task ReturnAsync(Product product)
        {
            await Task.Delay(10); // Simulate some latency
            Balance += product.Price;
        }
    }
}
