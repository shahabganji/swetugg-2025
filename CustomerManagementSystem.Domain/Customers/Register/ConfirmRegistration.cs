namespace CustomerManagementSystem.Domain.Customers.Register;

public sealed record ConfirmRegistration(Guid CustomerId);

public sealed class ConfirmRegistrationHandler(IEventStore eventStore)
{
    public Task Handle(ConfirmRegistration command)
    {
        throw new NotImplementedException();
    }
}
