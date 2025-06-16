using HospitalService.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HospitalService.Infrastructure.Data;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Repositories;
using HospitalService.Infrastructure.Repositories;

namespace HospitalService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton<ICurrentUserHelper, CurrentUserHelper>();

            var connectionString = configuration.GetConnectionString("Database");

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });

            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IServiceGroupRepository, ServiceGroupRepository>();
            services.AddScoped<IServiceGroupServiceRepository, ServiceGroupServiceRepository>();
            services.AddScoped<IDiseaseGroupRepository, DiseaseGroupRepository>();
            services.AddScoped<IDiseaseGroupServiceRepository, DiseaseGroupServiceRepository>();

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
