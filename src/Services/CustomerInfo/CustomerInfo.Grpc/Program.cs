using CustomerInfo.Grpc.Database;
using CustomerInfo.Grpc.Helpers;
using CustomerInfo.Grpc.Interceptors;
using CustomerInfo.Grpc.Mapping;
using CustomerInfo.Grpc.Services;
using Google.Protobuf.Collections;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.Grpc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
        TypeAdapterConfig.GlobalSettings.Default
            .UseDestinationValue(member => member.SetterModifier == AccessModifier.None &&
                                           member.Type.IsGenericType &&
                                           member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>));
        builder.Services.AddSingleton<IRegister, MapsterConfig>();

        // Add services to the container.
        builder.Services.AddGrpc();

        // Add DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database"!));
        });

        builder.Services.AddSingleton<ICurrentUserHelper, CurrentUserHelper>();
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcUserInterceptor>();
        });

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

        // Configure the HTTP request pipeline.
        app.MapGrpcService<PatientService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}