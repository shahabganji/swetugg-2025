namespace CustomerManagementSystem.Api.Shared;

public record StoredEvent(Guid StreamId, DateTime Timestamp, Event EventData);
