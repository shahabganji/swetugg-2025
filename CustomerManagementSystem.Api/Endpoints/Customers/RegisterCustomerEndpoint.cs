using CustomerManagementSystem.Api.Contracts;
using CustomerManagementSystem.Domain.Customers.Register;

namespace CustomerManagementSystem.Api.Endpoints.Customers;

internal static partial class CustomerEndpoints
{
    internal static class RegisterCustomerEndpoint
    {
        internal static void Configure(RouteGroupBuilder apiGroup)
        {
            apiGroup.MapPost("/customers", async (RegisterCustomerDto registration, RegisterCustomerHandler handler) =>
                {
                    var command = new RegisterCustomer(
                        Guid.CreateVersion7(), registration.FullName, registration.Email, registration.BirthDate);

                    await handler.Handle(command);

                    return Results.CreatedAtRoute("GetCustomer", new { id = command.CustomerId });
                })
                .WithName("RegisterCustomer");
        }
    }
}
