using HumanResource.Grpc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VaccinationReception.Application.Examinations.Handlers;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpsertExaminationResultEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestExaminationId = 1;
        private const int TestPatientId = 1;
        private const int TestDoctorId = 1;
        private const int TestUserId = 1;

        public UpsertExaminationResultEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
            SeedExaminationData();
        }

        [Fact]
        public async Task UpsertExaminationResult_WithValidPayload_ReturnsOk()
        {
            // Arrange
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>
            {
                new ExaminationTestResultUpsertDTO
                {
                    ExaminationId = TestExaminationId,
                    PatientId = TestPatientId,
                    Diagnose = "Healthy",
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 2,
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 3,
                    Conclusion = "Normal",
                    Note = "All good",
                    ExaminationResults = new List<ExaminationResultItem>
                    {
                        new ExaminationResultItem
                        {
                            ParameterName = "Glucose",
                            ResultValue = "90",
                            StandardValue = "70-110",
                            Unit = "mg/dL"
                        }
                    }
                }
            });

            // ApplicationUser gRPC mock for user information
            var userResponse = new ApplicationUserDetailModel
            {
                Id = TestUserId,
                Name = "Dr. Smith"
            };

            var userAsyncUnaryCall = new Grpc.Core.AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(userResponse),
                Task.FromResult(new Grpc.Core.Metadata()),
                () => Grpc.Core.Status.DefaultSuccess,
                () => new Grpc.Core.Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Grpc.Core.Metadata>(), null, default)
                .Returns(userAsyncUnaryCall);

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpsertExaminationResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpsertExaminationResult_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>());

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpsertExaminationResult_MissingDiagnose_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>
            {
                new ExaminationTestResultUpsertDTO
                {
                    ExaminationId = TestExaminationId,
                    PatientId = TestPatientId,
                    // Diagnose missing
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 2,
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 3,
                    Conclusion = "Normal",
                    Note = "All good",
                    ExaminationResults = new List<ExaminationResultItem>
                    {
                        new ExaminationResultItem
                        {
                            ParameterName = "Glucose",
                            ResultValue = "90",
                            StandardValue = "70-110",
                            Unit = "mg/dL"
                        }
                    }
                }
            });

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpsertExaminationResult_EmptyResults_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>());

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpsertExaminationResult_InvalidExaminationId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>
            {
                new ExaminationTestResultUpsertDTO
                {
                    ExaminationId = 999999, // Not exist
                    PatientId = TestPatientId,
                    Diagnose = "Healthy",
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 2,
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 3,
                    Conclusion = "Normal",
                    Note = "All good",
                    ExaminationResults = new List<ExaminationResultItem>
                    {
                        new ExaminationResultItem
                        {
                            ParameterName = "Glucose",
                            ResultValue = "90",
                            StandardValue = "70-110",
                            Unit = "mg/dL"
                        }
                    }
                }
            });

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpsertExaminationResult_MissingParameterNameInResult_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpsertExaminationResultCommand(new List<ExaminationTestResultUpsertDTO>
            {
                new ExaminationTestResultUpsertDTO
                {
                    ExaminationId = TestExaminationId,
                    PatientId = TestPatientId,
                    Diagnose = "Healthy",
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 2,
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 3,
                    Conclusion = "Normal",
                    Note = "All good",
                    ExaminationResults = new List<ExaminationResultItem>
                    {
                        new ExaminationResultItem
                        {
                            // ParameterName missing
                            ResultValue = "90",
                            StandardValue = "70-110",
                            Unit = "mg/dL"
                        }
                    }
                }
            });

            // Act
            var response = await _client.PostAsJsonAsync("/examination/results", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private void SeedExaminationData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == 1);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = 1,
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

            var examination = dbContext.Examinations.FirstOrDefault(e => e.Id == TestExaminationId);
            if (examination == null)
            {
                examination = new Examination
                {
                    Id = TestExaminationId,
                    ServiceId = 1,
                    ReceptionId = 1,
                    PatientId = TestPatientId,
                    RequestNumber = "REQ-001",
                    Diagnose = "Hypertension",
                    ReceptionTime = DateTime.UtcNow,
                    ExecutionTime = DateTime.UtcNow,
                    ReturnTime = DateTime.UtcNow,
                    PerformTechnicianId = 1,
                    PerformTechnicianName = "Dr. Smith",
                    SampleType = SampleType.Blood,
                    SampleQuality = SampleQualityLevel.High,
                    DoctorId = 1,
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