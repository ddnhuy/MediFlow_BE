using BuildingBlocks.Behaviors;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace VaccinationReception.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
            .AddApplicationService()
            .AddInfrastructureServices(builder.Configuration)
            .AddApiServices(builder.Configuration);

            builder.Services.AddFluentValidationAutoValidation();

            builder.Services
                .AddHealthChecks()
                .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

            builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(CreatePatientCommand).Assembly);
            });

            TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
            TypeAdapterConfig.GlobalSettings.Default
                .UseDestinationValue(member => member.SetterModifier == AccessModifier.None &&
                                               member.Type.IsGenericType &&
                                               member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>));
            builder.Services.AddSingleton<IRegister, MapsterConfig>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

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