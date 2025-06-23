using BuildingBlocks.Behaviors;
using FluentValidation.AspNetCore;
using FluentValidation;
using VaccinationReception.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Domain.IServiceClients;
using VaccinationReception.Infrastructure.ServiceClients;

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

            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            void ConfigureHttpClient<TInterface, TImplementation>(string baseUrlKey)
                where TInterface : class
                where TImplementation : class, TInterface
            {
                builder.Services.AddHttpClient<TInterface, TImplementation>(client =>
                {
                    var baseUrl = builder.Configuration[baseUrlKey];
                    if (string.IsNullOrWhiteSpace(baseUrl))
                        throw new InvalidOperationException($"Missing base URL for {baseUrlKey}");

                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .ConfigurePrimaryHttpMessageHandler(() => httpHandler);
            }

            ConfigureHttpClient<IHospitalServiceClient, HospitalServiceClient>("HospitalService:BaseUrl");
            ConfigureHttpClient<IInventoryServiceClient, InventoryServiceClient>("InventoryService:BaseUrl");

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