using CustomerManagementSystem.Domain.Customers.ConfirmRegistration;

namespace CustomerManagementSystem.Api.Endpoints.Customers;

internal static partial class CustomerEndpoints
{
    internal static class ConfirmRegistrationEndpoint
    {
        internal static void Configure(RouteGroupBuilder apiGroup)
        {
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
}
