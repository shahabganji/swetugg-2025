namespace CustomerManagementSystem.Domain.Customers.GetCustomer;

public record GetAllCustomers;

public sealed class GetAllCustomersHandler(IEventStore store)
{
    public async Task<IEnumerable<Customer>> Handle(GetAllCustomers _)
    {
        var streamIds = await store.GetStreamIds();

        var customers = new List<Customer>();

        foreach (var streamId in streamIds)
        {
            var stream = new EventStream<Customer>(store, streamId);
            var customer = await stream.GetEntity();
            customer.WhenSome(customers.Add);
        }

        return customers;
    }
}
