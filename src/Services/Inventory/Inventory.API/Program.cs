using HealthChecks.UI.Client;
using Inventory.API;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationService(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

app.UseApiServices();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

await app.UseMigrationAsync();

app.Run();

public partial class Program { }