using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Strings;
using NSubstitute;
using System.Text.Json;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllExaminationOfReceptionEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestPatientId = 1;
        private const int TestExaminationId1 = 1;
        private const int TestExaminationId2 = 2;

        public GetAllExaminationOfReceptionEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAllExaminationOfReception_WithValidReceptionId_ReturnsOkWithExaminations()
        {
            // Arrange
            SeedExaminationData();

            // Mock HospitalService response
            var hospitalServiceResponse = new List<ServiceDTO>
            {
                new ServiceDTO
                {
                    Id = 1,
                    ServiceCode = "BLOOD001",
                    ServiceName = "Blood Test",
                    UnitPrice = 150000m,
                    DepartmentId = 2,
                    ExaminationService = null,
                    ServiceTestParameters = new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceTestParameterDTO>()
                },
                new ServiceDTO
                {
                    Id = 2,
                    ServiceCode = "XRAY001",
                    ServiceName = "X-Ray Examination",
                    UnitPrice = 200000m,
                    DepartmentId = 2,
                    ExaminationService = null,
                    ServiceTestParameters = new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceTestParameterDTO>()
                }
            };

            _factory.HospitalServiceMock!
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(hospitalServiceResponse);

            // Act
            var response = await _client.GetAsync($"/reception/{TestReceptionId}/examination");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllExaminationOfReceptionQueryResponse>();
            result.Should().NotBeNull();
            result!.Examinations.Should().NotBeNull();
            result.Examinations.Should().HaveCount(2);

            result.Examinations.Should().Contain(e => e.ExaminationId == TestExaminationId1 && e.ServiceName == "Blood Test");
            result.Examinations.Should().Contain(e => e.ExaminationId == TestExaminationId2 && e.ServiceName == "X-Ray Examination");
        }

        [Fact]
        public async Task GetAllExaminationOfReception_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/reception/{TestReceptionId}/examination");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

            // Create first Examination if not exists
            var examination1 = dbContext.Examinations.FirstOrDefault(e => e.Id == TestExaminationId1);
            if (examination1 == null)
            {
                examination1 = new Examination
                {
                    Id = TestExaminationId1,
                    ServiceId = 1, // Blood Test
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
                dbContext.Examinations.Add(examination1);
            }

            // Create second Examination if not exists
            var examination2 = dbContext.Examinations.FirstOrDefault(e => e.Id == TestExaminationId2);
            if (examination2 == null)
            {
                examination2 = new Examination
                {
                    Id = TestExaminationId2,
                    ServiceId = 2, // X-Ray Examination
                    ReceptionId = TestReceptionId,
                    PatientId = TestPatientId,
                    RequestNumber = "REQ-002",
                    Diagnose = "Chest pain",
                    ReceptionTime = DateTime.Parse("2024-01-15T11:00:00Z").ToUniversalTime(),
                    ExecutionTime = DateTime.Parse("2024-01-15T11:30:00Z").ToUniversalTime(),
                    ReturnTime = DateTime.Parse("2024-01-15T12:00:00Z").ToUniversalTime(),
                    PerformTechnicianId = 102,
                    PerformTechnicianName = "Dr. Brown",
                    SampleType = null,
                    SampleQuality = null,
                    DoctorId = 202,
                    DoctorName = "Dr. Wilson",
                    Conclusion = "No abnormalities found",
                    Note = "Routine check",
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Examinations.Add(examination2);
            }

            dbContext.SaveChanges();
        }
    }
}