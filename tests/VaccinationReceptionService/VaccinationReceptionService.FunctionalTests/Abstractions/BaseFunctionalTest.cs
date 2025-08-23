using VaccinationReception.Application.Abstraction.InventoryMessaging;

namespace VaccinationReceptionService.FunctionalTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _grpcClientMock = factory._grpcClientMock;
            _mockInventoryService = factory.InventoryServiceMock;
        }

        protected HttpClient _client = new();
        protected PatientProtoServiceClient? _grpcClientMock;
        protected IInventoryService _mockInventoryService;
    }
}
