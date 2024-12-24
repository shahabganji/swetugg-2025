namespace CustomerManagementSystem.Domain;

public record StoredEvent(Guid StreamId, DateTime Timestamp, IEvent EventData);
