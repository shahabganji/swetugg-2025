namespace CustomerManagementSystem.WebApp.Customers;

internal sealed record Customer(Guid Id, string FullName, string Email, bool IsConfirmed);
