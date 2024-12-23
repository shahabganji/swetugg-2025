using CustomerManagementSystem.Api.Shared.Fx;

namespace CustomerManagementSystem.Api.Shared;

public interface IEventStore
{
    Task Append<TE>(TE @event) where TE : Event;
    Task<Maybe<TA>> Get<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
    Task<Maybe<TA>> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new();
}
