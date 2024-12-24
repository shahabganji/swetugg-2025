using System.Text.Json.Serialization;
using CustomerManagementSystem.Api.Customers;
using CustomerManagementSystem.Api.Shared.Fx;
using Microsoft.Azure.Cosmos;


namespace CustomerManagementSystem.Api.Shared;

public sealed class CosmosEventStore : IEventStore
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

    /// <summary>
    /// Appends an event to the end of the stream
    /// </summary>
    /// <param name="event">The event to be stored in the stream</param>
    public async Task Append(StoredEvent @event)
    {
        var storedEvent = new CosmosStoredEvent(@event.StreamId, @event.Timestamp, @event.EventData);
        // var customer = await GetEntity(@event.StreamId) ?? new();
        // customer.Apply(@event);

        var transactionalBatch = _container.CreateTransactionalBatch(new PartitionKey(storedEvent.StreamId.ToString()));

        // transactionalBatch.UpsertItem(customer);
        transactionalBatch.UpsertItem(storedEvent);

        _ = await transactionalBatch.ExecuteAsync(CancellationToken.None);

        Console.WriteLine($@"Event written to the database: {@event.StreamId}");

        // await _container.UpsertItemAsync<Event>(@event, new PartitionKey(@event.StreamId.ToString()));
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

    public Task SaveStream(CancellationToken cancellation)
    {
        return Task.CompletedTask;
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
