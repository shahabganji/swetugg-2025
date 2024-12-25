namespace CustomerManagementSystem.Domain.Customers.Register;

internal sealed partial record RegistrationConfirmed(Guid CustomerId) : IEvent<Customer>;
