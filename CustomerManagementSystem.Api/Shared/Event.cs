using System.Text.Json.Serialization;

namespace CustomerManagementSystem.Api.Shared;

public abstract partial record Event
{
    public abstract Guid StreamId { get; }
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    [JsonPropertyName("id")] public string Id => CreatedAtUtc.ToString("O");
}
