using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.CurrentUser;
using VaccinationReception.Application.Data;
using VaccinationReception.Infrastructure.Data;
using VaccinationReception.Infrastructure.Helpers;
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
            services.AddMessageBroker(configuration, typeof(DependencyInjection).Assembly, useCompetingConsumers: true);
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            return services;
        }
    }
}