namespace CustomerManagementSystem.Api.Customers;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(string email) : base($"Aggregate with email: {email} was not found")
    {
    }

    public AggregateNotFoundException(Guid customerId) : base($"Aggregate with id: {customerId} was not found")
    {
    }
}
