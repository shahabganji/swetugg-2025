using System.Reflection.Metadata;
using CustomerManagementSystem.Api.Customers;
using CustomerManagementSystem.Api.Customers.GetCustomer;
using CustomerManagementSystem.Api.Customers.Register;
using CustomerManagementSystem.Api.Shared;
using CustomerManagementSystem.Api.Shared.Serializers;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<GetCustomerHandler>();
builder.Services.AddScoped<RegisterCustomerHandler>();
builder.Services.AddScoped<IEventStore>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("CosmosDb");

    var cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
    {
        ApplicationName = "Swetugg - Roslyn",
        EnableContentResponseOnWrite = false,

        Serializer = new CosmosSystemTextJsonSerializer(),

        ApplicationPreferredRegions = ["Sweden Central"],
    });

    var database = cosmosClient.GetDatabase("Swetugg-Demo");
    var container = database.GetContainer("Customers");

    return new CosmosEventStore(container);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/customer", async (RegisterCustomer command, RegisterCustomerHandler handler) =>
    {
        await handler.Handle(command);
        return Results.CreatedAtRoute("GetCustomer", new { id = command.CustomerId });
    })
    .WithName("RegisterCustomer");

app.MapGet("/customer/{id}", async (Guid id, GetCustomerHandler handler) =>
    {
        var customer = await handler.Handle(new GetCustomer(id));
        return customer.Match(() => Results.NotFound(), Results.Ok);
    })
    .WithName("GetCustomer");

app.Run();
