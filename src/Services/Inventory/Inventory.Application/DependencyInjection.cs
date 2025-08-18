using HumanResource.Grpc;
using Inventory.Application.Services;
using OfficeOpenXml;

namespace Inventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            ExcelPackage.License.SetNonCommercialPersonal("Personal Use");
            services.AddScoped<IInventoryStatisticsExcelService, InventoryStatisticsExcelService>();
            services.AddScoped<IMedicineRevenueExcelService, MedicineRevenueExcelService>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFeatureManagement();

            services.AddGrpcClient<ApplicationUserProtoService.ApplicationUserProtoServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcSettings:HumanResourceUrl"]!);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return handler;
            });

            return services;
        }
    }
}
