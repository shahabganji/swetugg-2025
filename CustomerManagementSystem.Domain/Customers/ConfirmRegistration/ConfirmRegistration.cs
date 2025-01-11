using CustomerManagementSystem.Domain.Fx;

namespace CustomerManagementSystem.Domain.Customers.ConfirmRegistration;

public sealed record ConfirmRegistration(Guid CustomerId);

public sealed class ConfirmRegistrationHandler(IEventStore eventStore)
{
    public async Task<Maybe<Customer>> Handle(ConfirmRegistration command)
    {
        var stream = new EventStream<Customer>(eventStore, command.CustomerId);

        var customer = await stream.GetEntity();

        if (customer.IsNone)
        {
            throw new Exception("Customer not found");
        }

        if (customer.UnwrappedValue.IsRegistrationConfirmed)
            return customer;

        stream.Append(new RegistrationConfirmed(command.CustomerId));

        await eventStore.SaveStream(CancellationToken.None);

        return customer;
    }
}
