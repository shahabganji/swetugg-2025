namespace CustomerManagementSystem.Api.Shared;

public interface IEventStore
{
    Task Append<TE>(TE @event) where TE : Event;
    Task<TA?> Get<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
    Task<TA?> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
}
