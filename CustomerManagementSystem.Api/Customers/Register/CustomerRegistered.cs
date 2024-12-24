using CustomerManagementSystem.Api.Shared;

namespace CustomerManagementSystem.Api.Customers.Register;

public sealed record CustomerRegistered(Guid CustomerId, string FullName, string Email, DateTime DateOfBirth)
    : Event<Customer>;
