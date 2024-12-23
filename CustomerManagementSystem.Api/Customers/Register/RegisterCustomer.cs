using CustomerManagementSystem.Api.Shared;

namespace CustomerManagementSystem.Api.Customers.Register;

public sealed record RegisterCustomer(Guid CustomerId, string FullName, string Email, DateTime DateOfBirth);

public sealed class RegisterCustomerHandler(IEventStore eventStore)
{
    public async Task Handle(RegisterCustomer command)
    {
        var customer = await eventStore.Get<Customer>(command.CustomerId);

        if (customer.IsNone)
        {
            await eventStore.Append(
                new CustomerRegistered(command.CustomerId, command.FullName, command.Email, command.DateOfBirth));
        }
    }
}
