namespace Inventory.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            return services;
        }
    }
}
