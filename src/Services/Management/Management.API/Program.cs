using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Strings.Extensions;
using HealthChecks.UI.Client;
using Inventory.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using VaccinationReception.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// gRPC Services - HumanResource related
builder.Services
    .AddGrpcClient<ApplicationUserProtoService.ApplicationUserProtoServiceClient>(options =>
    {
        options.Address = new Uri(builder.Configuration["GrpcSettings:HumanResourceUrl"]!);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });

builder.Services
    .AddGrpcClient<RoleProtoService.RoleProtoServiceClient>(options =>
    {
        options.Address = new Uri(builder.Configuration["GrpcSettings:HumanResourceUrl"]!);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });

builder.Services
    .AddGrpcClient<DepartmentProtoService.DepartmentProtoServiceClient>(options =>
    {
        options.Address = new Uri(builder.Configuration["GrpcSettings:HumanResourceUrl"]!);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });

builder.Services
    .AddGrpcClient<DepartmentTypeProtoService.DepartmentTypeProtoServiceClient>(options =>
    {
        options.Address = new Uri(builder.Configuration["GrpcSettings:HumanResourceUrl"]!);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });

// gRPC Services - For Inventory and VaccinationReception (simpler configuration for production)
builder.Services.AddGrpcClient<InventoryProtoService.InventoryProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:InventoryUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
        }
    };
});

builder.Services.AddGrpcClient<VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:VaccinationReceptionUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
        }
    };
});

// Cross-Cutting Services
builder.Services.AddSeqLogging(serviceName: Assembly.GetExecutingAssembly().GetName().Name!);

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(
    Assembly.GetExecutingAssembly(),
    includeInternalTypes: true
);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(options => { });

app.UseHealthChecks(
    "/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program;
