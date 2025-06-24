using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.CurrentUser;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Infrastructure.Data;
using VaccinationReception.Infrastructure.Helpers;
using VaccinationReception.Infrastructure.Services.HospitalServiceMessaging;
using VaccinationReception.Infrastructure.Services.InventoryMessaging;

namespace VaccinationReception.Infrastructure
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
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly(), useCompetingConsumers: true);
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<ICurrentUserHelper, CurrentUserHelper>();
            return services;
        }
    }
}