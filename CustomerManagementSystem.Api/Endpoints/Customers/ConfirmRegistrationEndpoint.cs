using CustomerManagementSystem.Domain.Customers.GetCustomer;

namespace CustomerManagementSystem.Api.Endpoints.Customers;

internal static partial class CustomerEndpoints
{
    internal static class ConfirmRegistrationEndpoint
    {
        internal static void Configure(RouteGroupBuilder apiGroup)
        {
            apiGroup.MapPost("/customers/{id}", async (Guid id, GetCustomerHandler handler) =>
                {
                    var customer = await handler.Handle(new GetCustomer(id));
                    return customer.Match(() => Results.NotFound(), Results.Ok);
                })
                .WithName("GetCustomer");
        }
    }
}
