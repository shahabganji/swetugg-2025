using CustomerManagementSystem.Domain.Fx;

namespace CustomerManagementSystem.Domain;

public interface IEventStore
{
    void Append(StoredEvent @event);
    Task<IReadOnlyCollection<StoredEvent>> GetEvents(Guid streamId);
    Task SaveStream(CancellationToken cancellation);
    Task<Maybe<TA>> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
    Task<IEnumerable<Guid>> GetStreamIds();
}
