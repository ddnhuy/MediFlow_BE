using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using VaccinationReception.API;
using VaccinationReception.Application.Data;
using VaccinationReception.Infrastructure.Data;
using VaccinationReception.Domain.IServiceClients;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
        public IHospitalServiceClient? HospitalServiceClientMock { get; private set; }
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

                // Mock gRPC client
                //GrpcClientMock = Substitute.For<PatientProtoServiceClient>();
                //services.AddSingleton(GrpcClientMock);

                // Mock Hospital Service client
                HospitalServiceClientMock = Substitute.For<IHospitalServiceClient>();
                services.AddSingleton(HospitalServiceClientMock);
    
                // Get DbContext instance
                var serviceProvider = services.BuildServiceProvider();
                DbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new Task DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
