namespace Cornichon.Tests.CommerceExample.FakeCommerce
{
    public class Product
    {
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        
        public decimal Price { get; }

        public override bool Equals(object obj)
            => obj is Product p ? p.Name == Name && p.Price == Price : false;

        public override int GetHashCode() => (Name, Price).GetHashCode();
    }
}
