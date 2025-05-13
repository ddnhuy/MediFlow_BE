using Authentication.Business;
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCarter();

builder.Services
    .AddDatabase(builder.Configuration)
    .AddServices();

// gRPC Services
builder.Services.AddGrpcClient<ApplicationUserProtoService.ApplicationUserProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:HumanResourceUrl"]!);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

// Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services
    .AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.UseMigrationAsync();
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
