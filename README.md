Cornichon
=========

A super-simple library to write tests in C#, using a syntax that looks as much like
[Gherkin](https://cucumber.io/docs/reference) as possible, while remaining C#, and
without having to deal with nasty regular expressions.

```csharp
Scenario
    .Given(() => { jeff.bought_a("microwave").at(100); })
      .And(() => { jeff.has_a_receipt(); })
     .When(() => { jeff.returns_the("microwave"); })
     .Then(() => { jeff.should_have(100); });
```

That's less pretty than the Gherkin natural language, but it also reduces the need for
special tooling, in particular for debugging.

The second goal of Cornichon is to be NetStandard 2.0 with no dependencies other than .NET itself.

Implementation
--------------

The `Cornichon.Scenario` class provides a fluent API that exposes the classic Gherkin
steps: `Given`, `When`, `Then`, and `And` (no but).
All of these methods are actually the same implementation, which is to take the `Action`
passed as the argument, invoke that, and return the scenario object for chaining.

Testing with Cornichon
----------------------

Cornichon makes no assumption about the testing framework, libraries, or runner.
Bring your own.
In this document, I'll use xUnit, but you don't have to, and Cornichon certainly takes
no dependency on it.

It's typical to define the steps of your tests as closures, so state is managed
automatically within the context of your test method:

```csharp
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
}
```

But wait, except for the "scenario", "given", and stuff, this looks nothing like Gherkin!
You are right, it doesn't, yet.
That's because when writing tests with any Gherkin-based test framework, you have to start
by writing preferably reusable pieces of code that implement the sentences that you'll use
after the Gherkin keywords.
That code typically implements user actions that can then be used to construct scenarios.
This is no different, so let's refactor the code above.

First, we'll implement a test customer helper that implements possible customer actions:

```csharp
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
            var product = Order.Products.FirstOrDefault(product => product.Name == productName);
            if (product != null)
            {
                Customer.Return(product);
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
    }
}
```

Then we can use that class to rewrite our test in a much more readable way:

```csharp
/// <summary>
/// Refund item.
/// </summary>
public class RefundItem
{
    private readonly TestCustomer jeff = new TestCustomer();

    [Fact]
    public void CustomerReturningProductsCausesRefund()
        => Scenario
            .Given(() => { jeff.bought_a("microwave").at(100); })
              .And(() => { jeff.has_a_receipt(); })
             .When(() => { jeff.returns_the("microwave"); })
             .Then(() => { jeff.should_have(100); });
}
```

### Asynchronous tests

Since version 3.0.0, Cornichon allows for asynchronous actions.
Tests can still be run synchronously as before, using the same syntax, but it is
also possible to use asynchronous Lambdas to run tests asynchronously.
The scenario steps are run sequentially, and the results of the previous steps are
awaited before the next one is run.
It is possible to use a mix of synchronous and asynchronous steps.

If the `CustomerTestHelper` class is modified to have an asynchronous method:


```csharp
public async ValueTask asynchronously_returns_the(string productName)
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
```

And `CustomerTestHelper.Purchase` also gets one:

```csharp
public async ValueTask asynchronously_for(decimal price)
    => _customerTestHelper.Order = await Order.CreateAsync(new Product(_product, price));
```

Then the scenario can be run asynchronously as well:

```csharp
[Fact]
public async ValueTask AsynchronousCustomerReturningProductsCausesRefund()
    => await Scenario
        .Given(async () => { await jeff.bought_a("microwave").asynchronously_for(100); })
          .And(      () => {       jeff.has_a_receipt(); })
         .When(async () => { await jeff.asynchronously_returns_the("microwave"); })
         .Then(      () => {       jeff.should_have(100); });
```
