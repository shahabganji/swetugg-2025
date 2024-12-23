namespace CustomerManagementSystem.Api.Customers;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(string email) : base($"Customer with email: {email} was not found")
    {
    }

    public AggregateNotFoundException(Guid customerId) : base($"Customer with id: {customerId} was not found")
    {
    }
}
