using BuildingBlocks.Pagination;
using NSubstitute;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Domain.Models;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllExaminationHistoryEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestPatientId = 1;
        private const int TestExaminationId = 1;
        private const int TestReceptionId = 1;

        public GetAllExaminationHistoryEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAllExaminationHistory_WithValidToken_ReturnsOkWithPaginatedResult()
        {
            // Arrange
            SeedExaminationData();

            // Mock patient GRPC response
            var grpcPatient = new PatientDetailDTO
            {
                Id = TestPatientId,
                Code = "BN001",
                Name = "Nguyen Van A",
                PhoneNumber = "0123456789"
            };

            _patientGrpcClientMock
                .GetPatientAsync(TestPatientId, Arg.Any<CancellationToken>())
                .Returns(grpcPatient);

            var pageIndex = 1;
            var pageSize = 10;
            var searchTerm = "";

            // Act
            var response = await _client.GetAsync($"/examination/history?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllExaminationHistoryResponse>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllExaminationHistory_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/examination/history?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private void SeedExaminationData()
        {
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