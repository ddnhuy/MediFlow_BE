using BuildingBlocks.Strings.Extensions;
using HealthChecks.UI.Client;
using Inventory.API;
using Inventory.API.Services;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support HTTP/2 without TLS for gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    // Listen on port 8080 for HTTP requests
    options.ListenAnyIP(
        8080,
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        }
    );
});

builder.Services
    .AddApplicationService(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddSeqLogging(serviceName: Assembly.GetExecutingAssembly().GetName().Name!);

builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

builder.Services.AddGrpc();

var app = builder.Build();

app.UseApiServices();

app.MapGrpcService<InventoryService>();

app.UseHealthChecks(
    "/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

await app.UseMigrationAsync();

app.Run();

public partial class Program { }
