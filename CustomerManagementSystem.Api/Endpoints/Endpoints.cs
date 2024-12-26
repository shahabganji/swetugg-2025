using CustomerManagementSystem.Api.Endpoints.Customers;

namespace CustomerManagementSystem.Api.Endpoints;

internal static class Endpoints
{
    public static void MapCustomerEndpoints(this RouteGroupBuilder apiGroup)
    {
        CustomerEndpoints.GetEndpoints.Configure(apiGroup);
        CustomerEndpoints.RegisterCustomerEndpoint.Configure(apiGroup);
        CustomerEndpoints.ConfirmRegistrationEndpoint.Configure(apiGroup);
    }
}
