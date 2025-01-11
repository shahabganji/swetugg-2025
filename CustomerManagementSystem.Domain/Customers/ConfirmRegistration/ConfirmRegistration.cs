using CustomerManagementSystem.Domain.Fx;

namespace CustomerManagementSystem.Domain.Customers.ConfirmRegistration;

public sealed record ConfirmRegistration(Guid CustomerId);

public sealed class ConfirmRegistrationHandler(IEventStore eventStore)
{
    public Task<Maybe<Customer>> Handle(ConfirmRegistration command)
    {
        throw new NotImplementedException();
    }
}
