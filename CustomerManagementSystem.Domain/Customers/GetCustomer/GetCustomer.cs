using CustomerManagementSystem.Domain.Fx;

namespace CustomerManagementSystem.Domain.Customers.GetCustomer;

public sealed record GetCustomer(Guid Id);


public sealed class GetCustomerHandler(IEventStore store)
{
    public async Task<Maybe<Customer>> Handle(GetCustomer query)
    {
        var stream = new EventStream<Customer>(store, query.Id);
        var customer = await stream.GetEntity();
        return customer;
    }
}
