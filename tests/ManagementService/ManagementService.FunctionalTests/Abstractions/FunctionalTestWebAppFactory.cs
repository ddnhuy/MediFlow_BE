using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ManagementService.FunctionalTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>
    {
        public ApplicationUserProtoServiceClient? _grpcUserClientMock { get; internal set; }
        public DepartmentProtoServiceClient? _grpcDepartmentClientMock { get; internal set; }
        public RoleProtoServiceClient? _grpcRoleClientMock { get; internal set; }
        public DepartmentTypeProtoServiceClient? _grpcDepartmentTypeClientMock { get; internal set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            builder.ConfigureServices(services =>
            {
                // Mock gRPC client
                _grpcUserClientMock = Substitute.For<ApplicationUserProtoServiceClient>();
                _grpcDepartmentClientMock = Substitute.For<DepartmentProtoServiceClient>();
                _grpcRoleClientMock = Substitute.For<RoleProtoServiceClient>();
                _grpcDepartmentTypeClientMock = Substitute.For<DepartmentTypeProtoServiceClient>();

                services.AddSingleton(_grpcUserClientMock);
                services.AddSingleton(_grpcDepartmentClientMock);
                services.AddSingleton(_grpcRoleClientMock);
                services.AddSingleton(_grpcDepartmentTypeClientMock);
            });
        }
    }
}
