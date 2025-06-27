using BuildingBlocks.Pagination;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationPatient;
using VaccinationReception.Domain.Models;
using ListPatientsResponse = CustomerInfo.Grpc.Protos.ListPatientsResponse;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetListPostVaccinationPatientEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetListPostVaccinationPatientEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
            SeedData();
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!dbContext.Vaccinations.Any())
            {
                var reception = new Reception
                {
                    PatientId = 1,
                    ServiceTypeId = 1
                };
                dbContext.Receptions.Add(reception);
                dbContext.SaveChanges();

                var receptionVaccination = new ReceptionVaccination
                {
                    ReceptionId = reception.Id,
                    RequestNumber = "TESTCODE"
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
                dbContext.SaveChanges();

                var vaccination = new Vaccination
                {
                    PatientId = 1,
                    ObservationConfirmed = false,
                    ReceptionVaccinationId = receptionVaccination.Id
                };
                dbContext.Vaccinations.Add(vaccination);
                dbContext.SaveChanges();
            }
        }
      
        //[Fact]
        //public async Task GetListPostVaccinationPatient_WithValidParameters_ReturnsOk()
        //{
        //    // Arrange - Create mock response for IPatientGrpcClient
        //    var grpcResponse = new ListPatientsResponse
        //    {
        //        PageIndex = 1,
        //        PageSize = 999,
        //        TotalItem = 2,
        //        Data =
        //        {
        //            new PatientSummaryModel
        //            {
        //                Id = 1,
        //                Code = "BN100",
        //                Name = "Test Patient 1",
        //                IdentityCard = "01233434",
        //                PhoneNumber = "0123456789"
        //            },
        //            new PatientSummaryModel
        //            {
        //                Id = 2,
        //                Code = "BN101",
        //                Name = "Test Patient 2",
        //                IdentityCard = "01223456",
        //                PhoneNumber = "0123456789"
        //            }
        //        }
        //    };

        //    var asyncUnaryCall = new AsyncUnaryCall<ListPatientsResponse>(
        //    Task.FromResult(grpcResponse),
        //    Task.FromResult(new Metadata()),
        //    () => Status.DefaultSuccess,
        //    () => new Metadata(),
        //    () => { });

        //    _grpcClientMock?
        //        .ListPatientsAsync(Arg.Any<ListPatientsRequest>(), Arg.Any<Metadata>())
        //        .Returns(asyncUnaryCall);

        //    // Act
        //    var response = await _client.GetAsync("/vaccination/post-vaccination?patientName=John");

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //    var result = await response.Content.ReadFromJsonAsync<List<GetListPostVaccinationPatientQueryResult>>();
        //    result.Should().NotBeNull().And.NotBeEmpty();
        //    result!.Count.Should().Be(1);
        //    result.First().PatientName.Should().Be("John Doe");
        //    result.First().PatientCode.Should().Be("BN100");
        //    result.First().Gender.Should().Be("Nam"); // Male in Vietnamese
        //}

        // Case unauthorized access
        [Fact]
        public async Task GetListPostVaccinationPatient_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            // Act
            var response = await _client.GetAsync("/vaccination/post-vaccination?patientName=John");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

    }
}