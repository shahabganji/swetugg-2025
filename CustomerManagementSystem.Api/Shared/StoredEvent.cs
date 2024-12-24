using System.Text.Json.Serialization;

namespace CustomerManagementSystem.Api.Shared;

public sealed record StoredEvent(Guid StreamId, DateTime Timestamp, Event EventData)
{
    [JsonPropertyName("id")] public string Id => Timestamp.ToString("O");
    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
}