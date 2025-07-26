using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using VaccinationReception.API;
using VaccinationReception.Application.Data;
using VaccinationReception.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Serilog;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace HospitalFee.FunctionalTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        //  public PatientProtoServiceClient? GrpcClientMock { get; private set; }
        public IInventoryService InventoryServiceMock { get; private set; } = Substitute.For<IInventoryService>();

        public IHospitalService HospitalServiceMock { get; private set; } = Substitute.For<IHospitalService>();
        public ApplicationDbContext? DbContext { get; private set; }

        public async Task ResetDatabaseAsync()
        {
            if (DbContext is null) return;

            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.EnsureCreatedAsync();
        }

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

                services.RemoveAll<ISchedulerFactory>();
                services.RemoveAll<IScheduler>();
             
                services.RemoveAll<IHostedService>();
                services.RemoveAll<VaccinationReception.Application.Jobs.CleanupUnpaidItemsJob>();
                // Get DbContext instance
                var serviceProvider = services.BuildServiceProvider();
                DbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

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
