using CustomerManagementSystem.Api.Contracts;
using CustomerManagementSystem.Domain.Customers.GetCustomer;

namespace CustomerManagementSystem.Api.Endpoints.Customers;

internal static partial class CustomerEndpoints
{
    internal static class GetEndpoints
    {
        internal static void Configure(RouteGroupBuilder apiGroup)
        {
            apiGroup.MapGet("/customers", async (GetAllCustomersHandler handler) =>
                {
                    var customers = await handler.Handle(new GetAllCustomers());
                    return Results.Ok(customers.Select(c =>
                        new CustomerDto(c.CustomerId, c.FullName, c.Email, c.IsRegistrationConfirmed)));
                })
                .WithName("GetCustomers");

            apiGroup.MapGet("/customers/{id:guid}", async (Guid id, GetCustomerHandler handler) =>
                {
                    var customer = await handler.Handle(new GetCustomer(id));
                    return customer.Match(
                        () => Results.NotFound(),
                        c => Results.Ok(new CustomerDto(c.CustomerId, c.FullName, c.Email, c.IsRegistrationConfirmed)));
                })
                .WithName("GetCustomerWithId");
        }
    }
}
