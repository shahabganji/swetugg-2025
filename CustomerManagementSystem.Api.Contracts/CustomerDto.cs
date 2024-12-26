namespace CustomerManagementSystem.Api.Contracts;

public sealed record CustomerDto(Guid Id, string FullName, string Email, bool IsConfirmed)
{
    public bool IsConfirmed { get; set; } = IsConfirmed;
}
