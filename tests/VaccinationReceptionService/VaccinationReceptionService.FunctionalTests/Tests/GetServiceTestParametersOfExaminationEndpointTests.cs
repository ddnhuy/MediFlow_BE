using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Domain.Models;
using BuildingBlocks.Messaging.Contracts.HospitalService;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetServiceTestParametersOfExaminationEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestExaminationId = 1;
        private const int TestReceptionId = 1;
        private const int TestPatientId = 1;
        private const int TestServiceId = 1;

        public GetServiceTestParametersOfExaminationEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetServiceTestParametersOfExamination_WithValidId_ReturnsOkWithParameters()
        {
            // Arrange
            SeedExaminationData();

            // Mock HospitalService response
            var hospitalServiceResponse = new List<ServiceDTO>
            {
                new ServiceDTO
                {
                    Id = TestServiceId,
                    ServiceTestParameters = new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceTestParameterDTO>
                    {
                        new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceTestParameterDTO
                        {
                            ParameterName = "Glucose",
                            StandardValue = "70-110",
                            Unit = "mg/dL",
                            SpecimenType = "Blood",
                            EquipmentName = "Analyzer"
                        }
                    }
                }
            };

            _factory.HospitalServiceMock!
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(hospitalServiceResponse);

            // Act
            var response = await _client.GetAsync($"/examinations/{TestExaminationId}/service-test-parameters");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetServiceTestParametersOfExaminationResponse>();
            result.Should().NotBeNull();
            result!.ServiceTestParameters.Should().NotBeNullOrEmpty();
            result.ServiceTestParameters.First().ParameterName.Should().Be("Glucose");
        }

        [Fact]
        public async Task GetServiceTestParametersOfExamination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/examinations/{TestExaminationId}/service-test-parameters");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetServiceTestParametersOfExamination_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var response = await _client.GetAsync($"/examinations/{invalidId}/service-test-parameters");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private void SeedExaminationData()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = TestPatientId,
                    ReceptionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
                dbContext.SaveChanges();
            }

            // Create Examination if not exists
            var examination = dbContext.Examinations.FirstOrDefault(e => e.Id == TestExaminationId);
            if (examination == null)
            {
                examination = new Examination
                {
                    Id = TestExaminationId,
                    ServiceId = TestServiceId,
                    ReceptionId = TestReceptionId,
                    PatientId = TestPatientId,
                    RequestNumber = "REQ-001",
                    Diagnose = "Hypertension",
                    ReceptionTime = DateTime.UtcNow,
                    ExecutionTime = DateTime.UtcNow,
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 101,
                    PerformTechnicianName = "Dr. Smith",
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 201,
                    DoctorName = "Dr. Johnson",
                    Conclusion = "Patient shows normal blood pressure",
                    Note = "Follow up in 2 weeks",
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Examinations.Add(examination);
                dbContext.SaveChanges();
            }
        }
    }
}