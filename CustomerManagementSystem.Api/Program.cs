using System.Net.Mime;
using CustomerManagementSystem.Domain.Customers.GetCustomer;
using CustomerManagementSystem.Domain.Customers.Register;
using CustomerManagementSystem.CosmosDbStore.Extensions;
using CustomerManagementSystem.Domain.Extensions;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAntiforgery();

builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Wasm,
        // MediaTypeNames.Application.Json
    ]);
});

builder.Services.AddOpenApi();

builder.Services.AddCommandHandlers();
builder.Services.AddStore(builder.Configuration.GetConnectionString("CosmosDb")!);

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

// This is necessary to serve the blazor application files
app.UseBlazorFrameworkFiles();
// this is definitely necessary because the
// blazor application has some static files, that required to be served
app.UseStaticFiles();

// app.UseAntiforgery();


var apiGroup = app.MapGroup("/api");

apiGroup.MapPost("/customers", async (RegisterCustomer command, RegisterCustomerHandler handler) =>
    {
        await handler.Handle(command);
        return Results.CreatedAtRoute("GetCustomer", new { id = command.CustomerId });
    })
    .WithName("RegisterCustomer");

apiGroup.MapGet("/customers", async context =>
    {
        await Results.Ok(new object[]
        {
            new
            {
                Id = Guid.NewGuid(), FullName = "John Doe", Email = "johndoe@example.com", IsConfirmed = false
            },
            new
            {
                Id = Guid.NewGuid(), FullName = "Jane Doe", Email = "jane.doe@example.com", IsConfirmed = true
            },
        }).ExecuteAsync(context);
    })
    .WithName("GetCustomers");

apiGroup.MapGet("/customers/{id}", async (Guid id, GetCustomerHandler handler) =>
    {
        var customer = await handler.Handle(new GetCustomer(id));
        return customer.Match(() => Results.NotFound(), Results.Ok);
    })
    .WithName("GetCustomer");

// if the requested route does not exist, then route it to the index.html file
app.MapFallbackToFile("index.html");

app.Run();
