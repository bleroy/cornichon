using Cornichon.Tests.CommerceExample.FakeCommerce;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cornichon.Tests.CommerceExample
{
    public class CustomerTestHelper
    {
        public CustomerTestHelper()
        {
            Customer = new Customer
            {
                Balance = 0
            };
        }

        public Customer Customer { get; }
        public Order Order { get; set; }

        public Purchase bought_a(string product)
        {
            return new Purchase(product, this);
        }

        public void has_a_receipt() => Customer.Receipts.Add(Order.Receipt);

        public void returns_the(string productName)
        {
            if (Order != null)
            {
                Product product = Order.Products.FirstOrDefault(product => product.Name == productName);
                if (product != null)
                {
                    Customer.Return(product);
                }
            }
        }

        public async Task asynchronously_returns_the(string productName)
        {
            if (Order != null)
            {
                Product product = await Order.FetchProductAsync(productName);
                if (product != null)
                {
                    await Customer.ReturnAsync(product);
                }
            }
        }

        public void should_have(decimal amount) => Assert.Equal(amount, Customer.Balance);

        public class Purchase
        {
            private string _product;
            private CustomerTestHelper _customerTestHelper;

            public Purchase(string product, CustomerTestHelper customerTestHelper)
            {
                _product = product;
                _customerTestHelper = customerTestHelper;
            }

            public void at(decimal price)
                => _customerTestHelper.Order = new Order(new Product(_product, price));

            public async Task asynchronously_for(decimal price)
                => _customerTestHelper.Order = await Order.CreateAsync(new Product(_product, price));
        }
    }
}
