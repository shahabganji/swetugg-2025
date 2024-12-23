using CustomerManagementSystem.Api.Shared;
using CustomerManagementSystem.Api.Shared.Fx;

namespace CustomerManagementSystem.Api.Customers.GetCustomer;

internal sealed record GetCustomer(Guid id);


internal sealed class GetCustomerHandler(IEventStore store)
{
    public async Task<Maybe<Customer>> Handle(GetCustomer query)
    {
        var customer = await store.Get<Customer>(query.id);
        return customer;
    }
}
