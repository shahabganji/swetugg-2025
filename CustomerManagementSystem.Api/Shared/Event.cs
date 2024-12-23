using System.Text.Json.Serialization;
using CustomerManagementSystem.Api.Customers.Register;
using CustomerManagementSystem.Api.Customers.UpdateContactsInfo;

namespace CustomerManagementSystem.Api.Shared;

public abstract partial record Event
{
    public abstract Guid StreamId { get; }
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    [JsonPropertyName("id")] public string Id => CreatedAtUtc.ToString("O");
}


public abstract partial record Event<TA> where TA : IAmAggregateRoot, new()
{
    public abstract Guid StreamId { get; }
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    [JsonPropertyName("id")] public string Id => CreatedAtUtc.ToString("O");
}


[JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(EmailUpdated), nameof(EmailUpdated))]
public abstract partial record Event<TA> where TA : IAmAggregateRoot, new()
{
}
