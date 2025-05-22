using Inventory.API;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationService(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

await app.UseMigrationAsync();

app.Run();

public partial class Program { }