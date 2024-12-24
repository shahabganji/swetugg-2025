namespace CustomerManagementSystem.Domain;

public interface IAmAggregateRoot
{
    // When this is an abstract class and the other Apply methods are marked as private, 
    // the Apply method in the abstract class cannot find other Apply methods
    // hence, it ends up in an infinite recursive call
    // When using an interface, it throws an exception indicating that:
    // The corresponding "Apply method" is inaccessible due to its protection level
    public void Apply(Event @event);
}
