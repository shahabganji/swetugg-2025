using CustomerManagementSystem.CosmosDbStore.Serializers;
using CustomerManagementSystem.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.CosmosDbStore.Extensions;

public static class StoreServiceExtensions
{
    public static void AddStore(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEventStore>(_ =>
        {
            var cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
            {
                ApplicationName = "Swetugg - Roslyn",
                EnableContentResponseOnWrite = false,

                Serializer = new CosmosSystemTextJsonSerializer(),

                ApplicationPreferredRegions = ["Sweden Central"],
            });

            var database = cosmosClient.GetDatabase("Swetugg-Demo");
            var container = database.GetContainer("Endpoints");

            return new CosmosEventStore(container);
        });
    }
}
