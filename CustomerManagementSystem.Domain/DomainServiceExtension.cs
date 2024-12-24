using CustomerManagementSystem.Domain.Customers.GetCustomer;
using CustomerManagementSystem.Domain.Customers.Register;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.Domain;

public static class DomainServiceExtension
{
    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<GetCustomerHandler>();
        services.AddScoped<RegisterCustomerHandler>();
    }
}
