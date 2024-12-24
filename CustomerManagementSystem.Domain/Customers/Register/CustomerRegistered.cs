namespace CustomerManagementSystem.Domain.Customers.Register;

public sealed record CustomerRegistered(Guid CustomerId, string FullName, string Email, DateTime DateOfBirth)
    : IEvent<Customer>;
