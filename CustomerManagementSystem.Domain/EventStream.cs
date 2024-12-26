using CustomerManagementSystem.Domain.Fx;

namespace CustomerManagementSystem.Domain;

internal sealed class EventStream<TAggregate>(IEventStore store, Guid streamId)
    where TAggregate : IAmAggregateRoot, new()
{
    public void Append(IEvent @event)
    {
        var storedEvent = new StoredEvent(streamId, DateTime.UtcNow, @event);
        store.Append(storedEvent);
    }

    public async Task<Maybe<TAggregate>> GetEntity()
    {
        var events = await store.GetEvents(streamId);

        // TODO: Think: Maybe not using in here but using the exception in the confirmation scenario
        if (events.Count == 0)
        {
            return Maybe.None;
        }

        TAggregate aggregate = new();
        foreach (var evt in events)
        {
            aggregate.Apply(evt.EventData);
        }

        return aggregate;
    }
}
