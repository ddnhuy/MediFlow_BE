namespace ManagementService.FunctionalTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _grpcUserClientMock = factory._grpcUserClientMock;
            _grpcDepartmentClientMock = factory._grpcDepartmentClientMock;
            _grpcRoleClientMock = factory._grpcRoleClientMock;
            _grpcDepartmentTypeClientMock = factory._grpcDepartmentTypeClientMock;
        }

        protected HttpClient _client = new();
        protected ApplicationUserProtoServiceClient? _grpcUserClientMock;
        protected DepartmentProtoServiceClient? _grpcDepartmentClientMock;
        protected RoleProtoServiceClient? _grpcRoleClientMock;
        protected DepartmentTypeProtoServiceClient? _grpcDepartmentTypeClientMock;
    }
}
