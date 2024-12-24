using System.Text.Json.Serialization;
using CustomerManagementSystem.Domain.Customers.Register;
using CustomerManagementSystem.Domain.Customers.UpdateContactsInfo;

namespace CustomerManagementSystem.Domain;

public partial interface IEvent;


[JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CustomerRegistered), nameof(CustomerRegistered))]
[JsonDerivedType(typeof(EmailUpdated), nameof(EmailUpdated))]
public partial interface IEvent;

public interface IEvent<TA> : IEvent where TA : IAmAggregateRoot, new();
