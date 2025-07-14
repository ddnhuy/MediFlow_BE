using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Testcontainers.RabbitMq;

namespace VaccinationReceptionService.FunctionalTests.Abstractions
{
    public class CreateReceptionVaccinationFunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:4-management-alpine")
            .WithUsername("mediflow")
            .WithPassword("Mediflow@123")
            .WithPortBinding(5673, 5672) // RabbitMQ port
            .Build();

        public PatientProtoServiceClient? _grpcClientMock { get; internal set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Add appsettings.Test.json to override MessageBroker
                configBuilder.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices(services =>
            {
                // Configure test database
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });

                // Mock gRPC client
                _grpcClientMock = Substitute.For<PatientProtoServiceClient>();
                services.AddSingleton(_grpcClientMock);
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _rabbitMqContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _rabbitMqContainer.StopAsync();
        }
    }
}
