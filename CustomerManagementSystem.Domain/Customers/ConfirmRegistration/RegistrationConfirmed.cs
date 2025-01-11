namespace CustomerManagementSystem.Domain.Customers.ConfirmRegistration;

public sealed partial record RegistrationConfirmed(Guid CustomerId) : IEvent<Customer>;
