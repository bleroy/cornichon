using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cornichon.Tests.CommerceExample.FakeCommerce
{
    public class Order
    {
        public Order(Product product) => ProductQuantities.Add((1, product));

        public static async Task<Order> CreateAsync(Product product)
        {
            await Task.Delay(10); // Simulate some latency
            return new Order(product);
        }

        public IList<(int Quantity, Product Product)> ProductQuantities { get; }
            = new List<(int Quantity, Product Product)>();

        public IList<Product> Products => ProductQuantities.Select(pq => pq.Product).ToList();

        public Receipt Receipt { get; set; }

        public async Task<Product> FetchProductAsync(string name)
        {
            await Task.Delay(10); // Simulate some latency
            return Products.FirstOrDefault(p => p.Name == name);
        }
    }
}
