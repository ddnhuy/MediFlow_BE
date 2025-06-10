
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using HospitalService.Application;
using HospitalService.Application.Services.HospitalServices.Commands;
using HospitalService.Infrastructure;
using HospitalService.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
            .AddApplicationService(builder.Configuration)
            .AddInfrastructureServices(builder.Configuration)
            .AddApiServices(builder.Configuration);

            builder.Services
                .AddHealthChecks()
                .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    logger.LogInformation("Applying pending EF Core migrations...");
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migration completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during database migration.");
                    throw;
                }
            }

            app.UseApiServices();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
