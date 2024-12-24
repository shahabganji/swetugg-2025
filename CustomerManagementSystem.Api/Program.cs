using CustomerManagementSystem.Domain.Customers.GetCustomer;
using CustomerManagementSystem.Domain.Customers.Register;
using CustomerManagementSystem.CosmosDbStore.Extensions;
using CustomerManagementSystem.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCommandHandlers();
builder.Services.AddStore(builder.Configuration.GetConnectionString("CosmosDb")!);

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
