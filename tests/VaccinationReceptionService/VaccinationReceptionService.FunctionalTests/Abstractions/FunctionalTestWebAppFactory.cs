using Microsoft.Extensions.Logging;

namespace VaccinationReceptionService.FunctionalTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        public PatientProtoServiceClient? _grpcClientMock { get; internal set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
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
        }

        public new async Task DisposeAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _dbContainer.StopAsync(cts.Token);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout stopping container: {ex.Message}");
            }
        }
    }
}
