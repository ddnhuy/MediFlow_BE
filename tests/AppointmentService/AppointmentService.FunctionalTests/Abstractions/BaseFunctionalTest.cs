namespace AppointmentService.FunctionalTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _grpcUserClientMock = factory._grpcUserClientMock;
            _grpcDepartmentClientMock = factory._grpcDepartmentClientMock;
            _grpcPatientClientMock = factory._grpcPatientClientMock;
        }

        protected HttpClient _client = new();
        protected ApplicationUserProtoServiceClient? _grpcUserClientMock;
        protected DepartmentProtoServiceClient? _grpcDepartmentClientMock;
        protected PatientProtoServiceClient? _grpcPatientClientMock;
    }
}
