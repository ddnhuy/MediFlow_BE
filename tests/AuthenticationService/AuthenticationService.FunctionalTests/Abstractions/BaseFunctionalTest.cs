namespace AuthenticationService.FunctionalTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _grpcClientMock = factory._grpcClientMock;
        }

        protected HttpClient _client = new();
        protected ApplicationUserProtoServiceClient? _grpcClientMock;
    }
}
