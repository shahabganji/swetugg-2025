namespace CustomerManagementSystem.Domain.Customers.ConfirmRegistration;

public sealed record RegistrationConfirmed(Guid CustomerId) : IEvent<Customer>;
