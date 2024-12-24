namespace CustomerManagementSystem.Domain;

public abstract partial record Event
{
}

public abstract record Event<TA> : Event where TA : IAmAggregateRoot, new()
{
}
