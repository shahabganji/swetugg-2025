namespace CustomerManagementSystem.Domain.Customers.UpdateContactsInfo;

public sealed record EmailUpdated(Guid CustomerId, string Email) : IEvent<Customer>;
