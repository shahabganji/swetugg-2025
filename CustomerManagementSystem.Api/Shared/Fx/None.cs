namespace CustomerManagementSystem.Api.Shared.Fx;

public readonly struct None
{
    internal static readonly None Default = new();

    public override string ToString()
    {
        return "None";
    }
}