namespace CustomerManagementSystem.Domain.Customers.UpdateContactsInfo;

internal record EmailUpdated(Guid CustomerId, string Email) : IEvent<Customer>;
