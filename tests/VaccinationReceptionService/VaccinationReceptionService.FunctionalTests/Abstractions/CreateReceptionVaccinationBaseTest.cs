namespace VaccinationReceptionService.FunctionalTests.Abstractions
{
    public class CreateReceptionVaccinationBaseTest : IClassFixture<CreateReceptionVaccinationFunctionalTestWebAppFactory>
    {
        public CreateReceptionVaccinationBaseTest(CreateReceptionVaccinationFunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _grpcClientMock = factory._grpcClientMock;
        }

        protected HttpClient _client = new();
        protected PatientProtoServiceClient? _grpcClientMock;
    }
}
