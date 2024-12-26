namespace CustomerManagementSystem.Domain.Customers.Register;

public sealed partial record RegistrationConfirmed(Guid CustomerId) : IEvent<Customer>;
