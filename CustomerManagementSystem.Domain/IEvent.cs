using System.Text.Json.Serialization;
using CustomerManagementSystem.Domain.Customers.Register;
using CustomerManagementSystem.Domain.Customers.UpdateContactsInfo;

namespace CustomerManagementSystem.Domain;

[JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(EmailUpdated), nameof(EmailUpdated))]
[JsonDerivedType(typeof(CustomerRegistered), nameof(CustomerRegistered))]
public partial interface IEvent;
public interface IEvent<TA> : IEvent where TA : IAmAggregateRoot, new();
