using CustomerManagementSystem.Api.Contracts;
using CustomerManagementSystem.Domain.Customers.GetCustomer;
using CustomerManagementSystem.Domain.Customers.Register;

namespace CustomerManagementSystem.Api.Customers;

internal static class Endpoints
{
    public static void MapCustomersEndpoints(this RouteGroupBuilder apiGroup)
    {
        apiGroup.MapPost("/customers", async (RegisterCustomerDto registration, RegisterCustomerHandler handler) =>
            {
                var command = new RegisterCustomer(
                    Guid.CreateVersion7(), registration.FullName, registration.Email, registration.BirthDate);

                await handler.Handle(command);

                return Results.CreatedAtRoute("GetCustomer", new { id = command.CustomerId });
            })
            .WithName("RegisterCustomer");

        apiGroup.MapGet("/customers", async (GetAllCustomersHandler handler) =>
            {
                var customers = await handler.Handle(new GetAllCustomers());
                return Results.Ok(customers.Select(c =>
                    new CustomerDto(c.CustomerId, c.FullName, c.Email, c.IsRegistrationConfirmed)));
            })
            .WithName("GetCustomers");

        apiGroup.MapPost("/customers/{id}", async (Guid id, GetCustomerHandler handler) =>
            {
                var customer = await handler.Handle(new GetCustomer(id));
                return customer.Match(() => Results.NotFound(), Results.Ok);
            })
            .WithName("GetCustomer");

        apiGroup.MapPut("/customers/{id}/confirm", async (Guid id, ConfirmRegistrationHandler handler) =>
            {
                var customer = await handler.Handle(new ConfirmRegistration(id));
                return customer.Match(
                    () => Results.NotFound(),
                    _ => Results.Ok());
            })
            .WithName("ConfirmRegistration");
    }
}
