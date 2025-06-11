using HospitalService.API;
using HospitalService.FunctionalTests.Helpers;
using HospitalService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .WithAutoRemove(true)
            .Build();

        private string? _connectionString;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Configure test database
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

                // Register DbContext - the connection will be set in InitializeAsync
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });

            });
        }

        public async Task InitializeAsync()
        {
            // Start PostgreSQL container first
            await _dbContainer.StartAsync();
            _connectionString = _dbContainer.GetConnectionString();

            // Create the client which finalizes WebApplicationFactory initialization
            _ = CreateClient();

            // Now we can access Services
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<FunctionalTestWebAppFactory>>();

            try
            {
                // Drop and recreate database
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();

                // Seed test data
                DatabaseSeeder.SeedTestData(dbContext);

                logger.LogInformation("PostgreSQL test database initialized with test data");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the PostgreSQL test database");
                throw;
            }
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}
