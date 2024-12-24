using CustomerManagementSystem.Api.Shared;

namespace CustomerManagementSystem.Api.Customers.UpdateContactsInfo;

internal record EmailUpdated(Guid CustomerId, string Email) : Event<Customer>;
