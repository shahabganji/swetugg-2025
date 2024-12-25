namespace CustomerManagementSystem.Api.Contracts;

public record RegisterCustomerDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
}
