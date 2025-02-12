namespace CustomerManagementSystem.Domain.Customers.Register;

public sealed record RegisterCustomer(Guid CustomerId, string FullName, string Email, DateTime DateOfBirth);

public sealed class RegisterCustomerHandler(IEventStore eventStore)
{
    public async Task Handle(RegisterCustomer command)
    {
        var stream = new EventStream<Customer>(eventStore, command.CustomerId);

        stream.Append(
            new CustomerRegistered(command.CustomerId, command.FullName, command.Email, command.DateOfBirth));
        await eventStore.SaveStream(CancellationToken.None);
    }
}
