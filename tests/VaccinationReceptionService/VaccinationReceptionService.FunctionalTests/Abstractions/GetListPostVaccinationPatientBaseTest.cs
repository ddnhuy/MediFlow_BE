using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReceptionService.FunctionalTests.Abstractions
{
    public class GetListPostVaccinationPatientBaseTest : IClassFixture<GetListPostVaccinationPatientTestFactory>
    {
        public GetListPostVaccinationPatientBaseTest(GetListPostVaccinationPatientTestFactory factory)
        {
            _client = factory.CreateClient();
            _patientGrpcClientMock = factory.PatientGrpcClientMock;
        }

        protected HttpClient _client = new();
        protected IPatientGrpcClient _patientGrpcClientMock;
    }
}