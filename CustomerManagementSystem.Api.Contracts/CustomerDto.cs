namespace CustomerManagementSystem.Api.Contracts;

public sealed record CustomerDto(Guid Id, string FullName, string Email, bool IsConfirmed)
{
    public bool IsConfirmed { get; set; } = IsConfirmed;
    public Guid Id { get; set; } = Id;
    public string FullName { get; set; } = FullName;
    public string Email { get; set; } = Email;
}
