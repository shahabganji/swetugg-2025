using Microsoft.Azure.Cosmos;
using CosmosSystemTextJsonSerializer = CustomerManagementSystem.Api.Shared.Serializers.CosmosSystemTextJsonSerializer;


namespace CustomerManagementSystem.Api.Shared;

public sealed class CosmosEventStore : IEventStore
{
    private readonly Container _container;

    public CosmosEventStore(Container container)
    {
        _container = container;
    }

    /// <summary>
    /// Appends an event to the end of the stream
    /// </summary>
    /// <param name="event">The event to be stored in the stream</param>
    /// <typeparam name="TE">Generic type, because of serialization and inheritance</typeparam>
    public async Task Append<TE>(TE @event) where TE : Event
    {
        // var customer = await Get(@event.StreamId) ?? new();
        // customer.Apply(@event);

        var transactionalBatch = _container.CreateTransactionalBatch(new PartitionKey(@event.StreamId.ToString()));

        // transactionalBatch.UpsertItem(customer);
        transactionalBatch.UpsertItem<Event>(@event);

        _ = await transactionalBatch.ExecuteAsync(CancellationToken.None);

        Console.WriteLine($"Event written to the database: {@event.StreamId}");

        // await _container.UpsertItemAsync<Event>(@event, new PartitionKey(@event.StreamId.ToString()));
    }

    /// <summary>
    /// Gets the snapshot by applying the events in the stream
    /// </summary>
    /// <param name="streamId">The unique identifier of the stream</param>
    /// <returns></returns>
    public async Task<TA?> Get<TA>(Guid streamId) where TA : IAmAggregateRoot, new()
    {
        var streamIterator =
            _container.GetItemQueryIterator<Event>(
                new QueryDefinition($"SELECT * FROM c WHERE c.StreamId = '{streamId}' AND c.id <> '{streamId}'"));

        if (!streamIterator.HasMoreResults)
            return default;

        TA customer = new();

        while (streamIterator.HasMoreResults)
        {
            var readNext = await streamIterator.ReadNextAsync();

            foreach (var @event in readNext.Resource)
            {
                customer.Apply(@event);
            }
        }

        return customer;
    }

    /// <summary>
    /// Gets the snapshot using Point Read mechanism from ACD SDK
    /// </summary>
    /// <param name="streamId">The unique identifier of the stream</param>
    /// <returns></returns>
    public async Task<TA?> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new()
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
            return default;
        }
    }

    public FeedIterator<TA> GetChangeFeedIteratorFor<TA>(Guid streamId) where TA : IAmAggregateRoot, new()
    {
        FeedIterator<TA> iteratorForPartitionKey = _container.GetChangeFeedIterator<TA>(
            ChangeFeedStartFrom.Beginning(FeedRange.FromPartitionKey(new PartitionKey(streamId.ToString()))),
            ChangeFeedMode.LatestVersion);

        return iteratorForPartitionKey;
    }
}
