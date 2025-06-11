using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using CloudinaryDotNet;
using FileStorage.API.Database;
using FileStorage.API.Helpers;
using FileStorage.API.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"!));
});


IConfigurationSection cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
));

builder.Services.AddSingleton(cloudinary);
builder.Services.AddTransient<IMediaHelper, MediaHelper>();
builder.Services.AddTransient<IFileHelper, FileHelper>();

builder.Services.AddTransient<IFileRepository, FileRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserHelper, CurrentUserHelper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Cross-Cutting Services
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await dbContext.Database.MigrateAsync();

app.MapCarter();
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.UseAuthentication();

app.UseAuthorization();

app.Run();
