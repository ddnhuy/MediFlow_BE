using BuildingBlocks.Behaviors;
using FluentValidation.AspNetCore;
using FluentValidation;
using VaccinationReception.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VaccinationReception.API
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

            builder.Services.AddFluentValidationAutoValidation();

            builder.Services
                .AddHealthChecks()
                .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

            TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
            TypeAdapterConfig.GlobalSettings.Default
                .UseDestinationValue(member => member.SetterModifier == AccessModifier.None &&
                                               member.Type.IsGenericType &&
                                               member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>));
            builder.Services.AddSingleton<IRegister, MapsterConfig>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                    throw;
                }
            }

            app.UseApiServices();

            app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            //await app.UseMigrationAsync();

            app.Run();
        }
    }
}