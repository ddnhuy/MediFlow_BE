using Appointment.API.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AppointmentService.FunctionalTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        public ApplicationUserProtoServiceClient? _grpcUserClientMock { get; internal set; }
        public DepartmentProtoServiceClient? _grpcDepartmentClientMock { get; internal set; }
        public PatientProtoServiceClient? _grpcPatientClientMock { get; internal set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Configure test database
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });

                // Mock gRPC client
                _grpcUserClientMock = Substitute.For<ApplicationUserProtoServiceClient>();
                _grpcDepartmentClientMock = Substitute.For<DepartmentProtoServiceClient>();
                _grpcPatientClientMock = Substitute.For<PatientProtoServiceClient>();

                services.AddSingleton(_grpcUserClientMock);
                services.AddSingleton(_grpcDepartmentClientMock);
                services.AddSingleton(_grpcPatientClientMock);
            });
        }

        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }

        public new Task DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
