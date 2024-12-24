using System.Text.Json.Serialization;
using CustomerManagementSystem.Domain;
using CustomerManagementSystem.Domain.Customers;
using CustomerManagementSystem.Domain.Fx;
using Microsoft.Azure.Cosmos;

namespace CustomerManagementSystem.CosmosDbStore;

internal sealed class CosmosEventStore : IEventStore
{
    private sealed record CosmosStoredEvent(Guid StreamId, DateTime Timestamp, Event EventData)
        : StoredEvent(StreamId, Timestamp, EventData)
    {
        [JsonPropertyName("id")] public string Id => Timestamp.ToString("O");
        [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    }

    private readonly Container _container;

    public CosmosEventStore(Container container)
    {
        _container = container;
    }

    private readonly IDictionary<Guid, List<CosmosStoredEvent>> streams =
        new Dictionary<Guid, List<CosmosStoredEvent>>();

    /// <summary>
    /// Appends an event to the end of the stream
    /// </summary>
    /// <param name="event">The event to be stored in the stream</param>
    public void Append(StoredEvent @event)
    {
        if (streams.TryGetValue(@event.StreamId, out var storedEvents))
        {
            storedEvents.Add(new CosmosStoredEvent(@event.StreamId, @event.Timestamp, @event.EventData));
        }
        else
        {
            streams.Add(@event.StreamId,
                [new CosmosStoredEvent(@event.StreamId, @event.Timestamp, @event.EventData)]);
        }
    }

    /// <summary>
    /// Gets the snapshot by applying the events in the stream
    /// </summary>
    /// <param name="streamId">The unique identifier of the stream</param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<StoredEvent>> GetEvents(Guid streamId)
    {
        var streamIterator =
            _container.GetItemQueryIterator<StoredEvent>(
                new QueryDefinition($"SELECT * FROM c WHERE c.StreamId = '{streamId}' AND c.id <> '{streamId}'"));

        if (!streamIterator.HasMoreResults)
            return [];

        var events = new List<StoredEvent>();

        while (streamIterator.HasMoreResults)
        {
            var readNext = await streamIterator.ReadNextAsync();

            if (readNext.Count == 0)
                continue;

            events.AddRange(readNext.Resource);
        }

        return events;
    }

    public async Task SaveStream(CancellationToken cancellation)
    {
        foreach (var (key, events) in streams)
        {
            var streamId = key.ToString();
            var transactionalBatch = _container.CreateTransactionalBatch(new PartitionKey(streamId));

            foreach (var storedEvent in events)
            {
                transactionalBatch.UpsertItem(storedEvent);
                // await _container.UpsertItemAsync<Event>(@event, new PartitionKey(@event.StreamId.ToString()));   
            }

            _ = await transactionalBatch.ExecuteAsync(CancellationToken.None);
        }
    }

    /// <summary>
    /// Gets the snapshot using Point Read mechanism from ACD SDK
    /// </summary>
    /// <param name="streamId">The unique identifier of the stream</param>
    /// <returns></returns>
    public async Task<Maybe<TA>> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new()
    {
        try
        {
            var snapshot = await _container.ReadItemAsync<TA>(
                id: streamId.ToString(), new PartitionKey(streamId.ToString()), new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                });

            return snapshot.Resource;
        }
        catch
        {
            throw new AggregateNotFoundException(streamId);
        }
    }
}
