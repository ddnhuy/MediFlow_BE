using BuildingBlocks.Strings;
using System.Text.Json;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPatientExaminationDetailEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestExaminationId = 1;
        private const int TestReceptionId = 1;
        private const int TestPatientId = 1;

        public GetPatientExaminationDetailEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetPatientExaminationDetail_WithValidId_ReturnsOkWithExaminationDetails()
        {
            // Arrange
            SeedExaminationData();

            // Act
            var response = await _client.GetAsync($"/examination/{TestExaminationId}/patient-detail");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientExaminationDetailQueryResponse>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientExaminationDetail_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/examination/{TestExaminationId}/patient-detail");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPatientExaminationDetail_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var response = await _client.GetAsync($"/examination/{invalidId}/patient-detail");

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
                    ServiceId = 1,
                    ReceptionId = TestReceptionId,
                    PatientId = TestPatientId,
                    RequestNumber = "REQ-001",
                    Diagnose = "Hypertension",
                    ReceptionTime = DateTime.Parse("2024-01-15T09:00:00Z").ToUniversalTime(),
                    ExecutionTime = DateTime.Parse("2024-01-15T09:30:00Z").ToUniversalTime(),
                    ReturnTime = DateTime.Parse("2024-01-15T10:00:00Z").ToUniversalTime(),
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