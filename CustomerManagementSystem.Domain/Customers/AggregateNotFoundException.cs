namespace CustomerManagementSystem.Domain.Customers;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(Guid customerId) : base($"Aggregate with id: {customerId} was not found")
    {
    }
}
