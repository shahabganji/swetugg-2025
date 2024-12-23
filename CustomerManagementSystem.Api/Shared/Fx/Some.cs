namespace CustomerManagementSystem.Api.Shared.Fx;

public readonly struct Some<T>
{
    internal T Value { get; }

    internal Some(T value)
    {
        if (value == null)
            throw new ArgumentNullException();

        this.Value = value;
    }
}