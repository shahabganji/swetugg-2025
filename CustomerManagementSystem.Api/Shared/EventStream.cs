using CustomerManagementSystem.Api.Shared.Fx;

namespace CustomerManagementSystem.Api.Shared;

internal sealed class EventStream<TAggregate>(IEventStore store, Guid streamId)
    where TAggregate : IAmAggregateRoot, new()
{
    public void Append(Event @event)
    {
        var storedEvent = new StoredEvent(streamId, DateTime.UtcNow, @event);
        store.Append(storedEvent);
    }
    
    public async Task<Maybe<TAggregate>> GetEntity()
    {
        var events = await store.GetEvents(streamId);

        if (events.Count == 0)
        {
            // Replace this with throw exception for demo purposes
            return Maybe.None;
        }
        
        TAggregate aggregate = new ();
        foreach (var evt in events)
        {
            aggregate.Apply(evt.EventData);
        }

        return aggregate;
    }
}
