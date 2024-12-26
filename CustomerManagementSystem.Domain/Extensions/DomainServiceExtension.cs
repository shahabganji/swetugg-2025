using CustomerManagementSystem.Domain.Customers.GetCustomer;
using CustomerManagementSystem.Domain.Customers.Register;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.Domain.Extensions;

public static class DomainServiceExtension
{
    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<GetCustomerHandler>();
        services.AddScoped<GetAllCustomersHandler>();
        
        services.AddScoped<RegisterCustomerHandler>();
        services.AddScoped<ConfirmRegistrationHandler>();
    }
}
