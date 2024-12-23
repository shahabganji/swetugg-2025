using CustomerManagementSystem.Api.Shared;

namespace CustomerManagementSystem.Api.Customers.Register;

public sealed record RegisterCustomer(Guid CustomerId, string FullName, string Email, DateTime DateOfBirth);

public sealed class RegisterCustomerHandler(IEventStore eventStore)
{
    public async Task Handle(RegisterCustomer command)
    {
        //TODO: it always returns an instance of a customer, even if it does not exists
        var customer = await eventStore.Get<Customer>(command.CustomerId);

        if (customer?.CustomerId == command.CustomerId)
            return;

        await eventStore.Append(
            new CustomerRegistered(command.CustomerId, command.FullName, command.Email, command.DateOfBirth));
    }
}
