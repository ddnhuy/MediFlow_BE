using HumanResource.Grpc;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;

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

        public IInventoryService InventoryServiceMock { get; private set; } = Substitute.For<IInventoryService>();

        public IHospitalService HospitalServiceMock { get; private set; } = Substitute.For<IHospitalService>();

        public PatientProtoServiceClient? _grpcClientMock { get; internal set; }

        public ApplicationUserProtoService.ApplicationUserProtoServiceClient ApplicationUserProtoMock { get; private set; } =
    Substitute.For<ApplicationUserProtoService.ApplicationUserProtoServiceClient>();

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

                // Remove and replace ApplicationUserProtoService client
                services.RemoveAll<ApplicationUserProtoService.ApplicationUserProtoServiceClient>();
                services.AddSingleton(ApplicationUserProtoMock);

                // Mock HttpContextAccessor if needed
                services.AddSingleton(_ => {
                    var mockHttpContext = Substitute.For<IHttpContextAccessor>();
                    var context = new DefaultHttpContext();

                    // Add test claims
                    var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "Doctor")
                    });
                    context.User = new ClaimsPrincipal(identity);

                    mockHttpContext.HttpContext.Returns(context);
                    return mockHttpContext;
                });

                // Replace real inventory service with mock
                services.RemoveAll<IInventoryService>();
                services.AddSingleton(InventoryServiceMock);

                services.RemoveAll<IHospitalService>();
                services.AddSingleton(HospitalServiceMock);

                // Disable actual MassTransit RabbitMQ connection
                services.AddMassTransitTestHarness(cfg =>
                {
                    // Configure in-memory transport instead of RabbitMQ
                    cfg.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                });

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
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
                await _dbContainer.StopAsync(cts.Token);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout stopping container: {ex.Message}");
            }
            Log.CloseAndFlush();
        }
    }
}
