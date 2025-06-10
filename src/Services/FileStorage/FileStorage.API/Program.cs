using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using CloudinaryDotNet;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCarter();

IConfigurationSection cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
));
builder.Services.AddSingleton(cloudinary);
builder.Services.AddTransient<IMediaHelper, MediaHelper>();

// Cross-Cutting Services
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
