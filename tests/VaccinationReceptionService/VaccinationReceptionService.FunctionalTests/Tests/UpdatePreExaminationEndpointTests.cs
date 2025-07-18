using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Commands.UpdatePreExaminationResult;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdatePreExaminationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionVaccinationId = 1;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;

        public UpdatePreExaminationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
            SeedData();
        }

        private void SeedData()
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
                    PatientId = 1,
                    ReceptionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create ReceptionVaccination if not exists
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = false,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    //IsConfirmed = false,
                    IsPreExaminationTesting = false, // Initially false, will be updated to true
                    TestResultEntry = null, // Initially null, will be updated
                    VaccinationTestDate = null, // Initially null, will be updated
                    DoctorId = TestDoctorId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task UpdatePreExamination_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "Patient passed pre-examination testing. No contraindications found."
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdatePreExaminationResponse>();
            result.Should().NotBeNull();
            result!.IsSucess.Should().BeTrue();

            // Verify that the vaccination was updated in the database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.TestResultEntry.Should().Be("Patient passed pre-examination testing. No contraindications found.");
            updatedVaccination.IsPreExaminationTesting.Should().BeTrue();
            updatedVaccination.VaccinationTestDate.Should().NotBeNull();
            updatedVaccination.VaccinationTestDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task UpdatePreExamination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "Test result"
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdatePreExamination_WithNonExistentId_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentId = 9999;
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: nonExistentId,
                TestEntryResult: "Test result"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{nonExistentId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdatePreExamination_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "Test result"
            );
            var differentId = 9999;

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{differentId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Reception Vaccination ID mismatch");
        }

        [Fact]
        public async Task UpdatePreExamination_WithEmptyTestResult_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "" // Empty test result
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePreExamination_WithNullTestResult_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: null! // Null test result
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePreExamination_WithInvalidReceptionVaccinationId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: 0, // Invalid ID
                TestEntryResult: "Test result"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/0/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePreExamination_UpdatesExistingPreExaminationData_ReturnsSuccess()
        {
            // Arrange - First update
            var firstCommand = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "Initial test result"
            );

            await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", firstCommand);

            // Arrange - Second update
            var secondCommand = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: "Updated test result after re-evaluation"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", secondCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdatePreExaminationResponse>();
            result.Should().NotBeNull();
            result!.IsSucess.Should().BeTrue();

            // Verify that the vaccination was updated with the new data
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.TestResultEntry.Should().Be("Updated test result after re-evaluation");
            updatedVaccination.IsPreExaminationTesting.Should().BeTrue();
            updatedVaccination.VaccinationTestDate.Should().NotBeNull();
        }

        [Theory]
        [InlineData("Patient passed all pre-examination tests")]
        [InlineData("Patient has mild allergy to vaccine components")]
        [InlineData("Patient requires special monitoring during vaccination")]
        [InlineData("Patient cleared for vaccination with precautions")]
        public async Task UpdatePreExamination_WithVariousTestResults_ReturnsSuccess(string testResult)
        {
            // Arrange
            var command = new UpdatePreExaminationCommand(
                ReceptionVaccinationId: TestReceptionVaccinationId,
                TestEntryResult: testResult
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/pre-examination/{TestReceptionVaccinationId}/result", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdatePreExaminationResponse>();
            result.Should().NotBeNull();
            result!.IsSucess.Should().BeTrue();

            // Verify the test result was saved correctly
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.TestResultEntry.Should().Be(testResult);
        }
    }
}