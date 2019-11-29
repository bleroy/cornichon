using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!Receipts.Any(receipt => receipt.Order.Products.Contains(product)))
            {
                throw new InvalidOperationException($"Customer doesn't have a receipt for the product {product.Name}.");
            }
            Balance += product.Price;
        }

        public async Task ReturnAsync(Product product)
        {
            await Task.Delay(10); // Simulate some latency
            Return(product);
        }
    }
}
