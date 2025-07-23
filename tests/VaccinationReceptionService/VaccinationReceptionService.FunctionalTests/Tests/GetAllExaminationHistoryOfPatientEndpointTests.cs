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
    public class GetAllExaminationHistoryOfPatientEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestPatientId = 1;
        private const int TestExaminationId = 1;
        private const int TestReceptionId = 1;

        public GetAllExaminationHistoryOfPatientEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAllExaminationHistoryOfPatient_WithValidToken_ReturnsOkWithHistory()
        {
            // Arrange
            SeedExaminationData();

            // Mock patient GRPC response
            var grpcPatient = new PatientDetailDTO
            {
                Id = TestPatientId,
                Code = "BN001",
                Name = "Nguyen Van A",
                DOB = new DateTime(1990, 1, 1),
                PhoneNumber = "0123456789"
            };
            _patientGrpcClientMock
                .GetPatientAsync(TestPatientId, Arg.Any<CancellationToken>())
                .Returns(grpcPatient);

            // Mock hospital service for service name
            var hospitalService = _factory.HospitalServiceMock;
            hospitalService
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceName = "Blood Test"
                    }
                });

            // Act
            var response = await _client.GetAsync($"/examination/history/patient/{TestPatientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllExaminationHistoryOfPatientResponse>();
            result.Should().NotBeNull();
            result!.PatientId.Should().Be(TestPatientId);
            result.PatientName.Should().Be("Nguyen Van A");
            result.ExaminationHistory.Should().NotBeNullOrEmpty();
            result.ExaminationHistory.First().ServiceName.Should().Be("Blood Test");
        }

        [Fact]
        public async Task GetAllExaminationHistoryOfPatient_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/examination/history/patient/{TestPatientId}");

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