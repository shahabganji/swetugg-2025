namespace CustomerManagementSystem.Domain;

public partial interface IEvent;
public interface IEvent<TA> : IEvent where TA : IAmAggregateRoot, new();
