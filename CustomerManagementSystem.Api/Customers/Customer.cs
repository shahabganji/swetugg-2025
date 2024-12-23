using System.Text.Json.Serialization;
using CustomerManagementSystem.Api.Customers.Register;
using CustomerManagementSystem.Api.Customers.UpdateContactsInfo;
using CustomerManagementSystem.Api.Shared;

namespace CustomerManagementSystem.Api.Customers;

public sealed partial class Customer : IAmAggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    
    public void Apply(Event @event)
    {
        ((dynamic)this).Apply((dynamic)@event);
    }
    
    private partial void Apply(CustomerRegistered @event)
    {
        CustomerId = @event.CustomerId;
        FullName = @event.FullName;
        Email = @event.Email;
        DateOfBirth = @event.DateOfBirth;
    }

    private partial void Apply(EmailUpdated @event)
    {
        Email = @event.Email;
    }

    public string StreamId => CustomerId.ToString();
    [JsonPropertyName("pk")] public string Pk => CustomerId.ToString();
    [JsonPropertyName("id")] public string Id => CustomerId.ToString();
}


// auto-generated
public sealed partial class Customer
{
    // it has to be public if the Apply method is going to be on an abstract class/interface
    private partial void Apply(CustomerRegistered @event);
}
