using CustomerManagementSystem.Api.Shared.Fx;

namespace CustomerManagementSystem.Api.Shared;

public interface IEventStore
{
    Task Append(StoredEvent @event);
    Task<IReadOnlyCollection<StoredEvent>> GetEvents(Guid streamId);
    Task SaveStream(CancellationToken cancellation);
    Task<Maybe<TA>> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
}
