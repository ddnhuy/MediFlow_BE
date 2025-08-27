using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using MediatR;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CloseReceptionWithIssueEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionId = 3001;
        private const int TestReceptionVaccinationId = 4001;
        private const int TestMedicineId = 1;
        private const int TestDoctorId = 1;

        public CloseReceptionWithIssueEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithValidData_ReturnsSuccess()
        {
            // Arrange
            SeedValidReceptionData();

            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: TestReceptionId,
                IssueNote: "Test issue note - system malfunction"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{TestReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CloseReceptionWithIssueResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify database changes
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updatedReception = await dbContext.Receptions
                .FirstOrDefaultAsync(r => r.Id == TestReceptionId);

            updatedReception.Should().NotBeNull();
            updatedReception!.HasIssue.Should().BeTrue();
            updatedReception.IssueNote.Should().Be("Test issue note - system malfunction");
            updatedReception.IsVaccinationTodayConfirmed.Should().BeTrue();
            updatedReception.IssueDate.Should().NotBeNull();
            updatedReception.IssueDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }     

        [Fact]
        public async Task CloseReceptionWithIssue_WithNonExistentReception_ReturnsBadRequest()
        {
            // Arrange
            const int nonExistentReceptionId = 99999;
            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: nonExistentReceptionId,
                IssueNote: "Test issue note"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{nonExistentReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithCancelledReception_ReturnsBadRequest()
        {
            // Arrange
            SeedCancelledReceptionData();

            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: TestReceptionId,
                IssueNote: "Test issue note"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{TestReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithEmptyIssueNote_ReturnsBadRequest()
        {
            // Arrange

            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: TestReceptionId,
                IssueNote: ""
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{TestReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: TestReceptionId,
                IssueNote: "Test issue note"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{TestReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithInvalidReceptionId_ReturnsBadRequest()
        {
            // Arrange
            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: 0,
                IssueNote: "Test issue note"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/0", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private void SeedValidReceptionData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean up existing data
            var existingReception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (existingReception != null)
            {
                dbContext.Receptions.Remove(existingReception);
            }

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = DateTime.UtcNow,
                IsVaccinationTodayConfirmed = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.Receptions.Add(reception);
            dbContext.SaveChanges();
        }

        private void SeedReceptionDataWithIncompleteVaccinations()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean up existing data
            var existingReception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (existingReception != null)
            {
                dbContext.Receptions.Remove(existingReception);
            }

            var existingReceptionVaccination = dbContext.ReceptionVaccinations.FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (existingReceptionVaccination != null)
            {
                dbContext.ReceptionVaccinations.Remove(existingReceptionVaccination);
            }

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = DateTime.UtcNow,
                IsVaccinationTodayConfirmed = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            var receptionVaccination = new ReceptionVaccination
            {
                Id = TestReceptionVaccinationId,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 2, // Need 2 doses but will only complete 1
                ScheduledDate = DateTime.UtcNow.Date, // Today
                AppointmentDate = DateTime.UtcNow.Date,
                RequestNumber = "REQ-TEST-001",
                UnitPrice = 100.00m,
                DoctorId = TestDoctorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1,
                IsCancelled = false
            };

            // Create one completed vaccination out of 2 needed
            var vaccination = new Vaccination
            {
                ReceptionVaccinationId = TestReceptionVaccinationId,
                PatientId = TestPatientId,
                MedicineId = TestMedicineId,
                MedicineName = "Test Vaccine",
                DoctorId = TestDoctorId,
                IsConfirmed = true,
                VaccinationDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            dbContext.Receptions.Add(reception);
            dbContext.ReceptionVaccinations.Add(receptionVaccination);
            dbContext.Vaccinations.Add(vaccination);
            dbContext.SaveChanges();
        }

        private void SeedCancelledReceptionData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean up existing data
            var existingReception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (existingReception != null)
            {
                dbContext.Receptions.Remove(existingReception);
            }

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = DateTime.UtcNow,
                IsVaccinationTodayConfirmed = false,
                IsCancelled = true, // Cancelled reception
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.Receptions.Add(reception);
            dbContext.SaveChanges();
        }

        private void SeedAlreadyClosedReceptionData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean up existing data
            var existingReception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (existingReception != null)
            {
                dbContext.Receptions.Remove(existingReception);
            }

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = DateTime.UtcNow,
                IsVaccinationTodayConfirmed = true, // Already confirmed
                IssueNote = "Previous issue", // Has issue note
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.Receptions.Add(reception);
            dbContext.SaveChanges();
        }

        private void SetupInventoryServiceMock()
        {
            var medicineInfo = new GetMedicineInformationResponse
            {
                MedicineId = TestMedicineId,
                MedicineName = "Test Vaccine",
                IsRequiredTestingBeforeUse = false,
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);
        }

        [Fact]
        public async Task CloseReceptionWithIssue_WithReactionInCompletedVaccination_MarksReceptionVaccinationAsHasIssue()
        {
            // Arrange
            SeedReceptionDataWithReactionInCompletedVaccination();
            SetupInventoryServiceMock();
            SetupPublisherMock();

            var command = new CloseReceptionWithIssueCommand(
                ReceptionId: TestReceptionId,
                IssueNote: "Patient had adverse reaction",
                ReScheduleDate: DateTime.UtcNow.Date.AddDays(7)
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/closing-reception/reception/{TestReceptionId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CloseReceptionWithIssueResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify database changes
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updatedReceptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            updatedReceptionVaccination.Should().NotBeNull();
            updatedReceptionVaccination!.HasIssue.Should().BeTrue(); // Should be marked as HasIssue because completed dose had reaction
            updatedReceptionVaccination.IssueNote.Should().Contain("Patient had adverse reaction");
            updatedReceptionVaccination.ScheduledDate.Date.Should().Be(DateTime.UtcNow.Date.AddDays(7)); // Should be rescheduled
        }

        private void SeedReceptionDataWithReactionInCompletedVaccination()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean up existing data
            CleanupTestData(dbContext);

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = DateTime.UtcNow,
                IsVaccinationTodayConfirmed = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            var receptionVaccination = new ReceptionVaccination
            {
                Id = TestReceptionVaccinationId,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 2, // Need 2 doses
                ScheduledDate = DateTime.UtcNow.Date, // Today
                AppointmentDate = DateTime.UtcNow.Date,
                RequestNumber = "REQ-TEST-001",
                UnitPrice = 100.00m,
                DoctorId = TestDoctorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1,
                IsCancelled = false
            };

            // Create one completed vaccination with reaction
            var vaccination = new Vaccination
            {
                ReceptionVaccinationId = TestReceptionVaccinationId,
                PatientId = TestPatientId,
                MedicineId = TestMedicineId,
                MedicineName = "Test Vaccine",
                DoctorId = TestDoctorId,
                IsConfirmed = true,
                HasReaction = true, // HAS REACTION - this should trigger HasIssue = true
                VaccinationDate = DateTime.UtcNow,
                DoseNumber = 1,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            dbContext.Receptions.Add(reception);
            dbContext.ReceptionVaccinations.Add(receptionVaccination);
            dbContext.Vaccinations.Add(vaccination);
            dbContext.SaveChanges();
        }

        private void CleanupTestData(ApplicationDbContext dbContext)
        {
            // Clean up existing data
            var existingVaccinations = dbContext.Vaccinations.Where(v => v.ReceptionVaccinationId == TestReceptionVaccinationId);
            dbContext.Vaccinations.RemoveRange(existingVaccinations);

            var existingReceptionVaccination = dbContext.ReceptionVaccinations.FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (existingReceptionVaccination != null)
            {
                dbContext.ReceptionVaccinations.Remove(existingReceptionVaccination);
            }

            var existingReception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (existingReception != null)
            {
                dbContext.Receptions.Remove(existingReception);
            }

            dbContext.SaveChanges();
        }

        private void SetupPublisherMock()
        {
            var patientResponse = new PatientDetailModel
            {
                Id = TestPatientId,
                Code = "BN001",
                Name = "Test Patient",
                Email = "test@example.com",
                PhoneNumber = "0123456789"
            };

            var patientAsyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(patientResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock?
                .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
                .Returns(patientAsyncUnaryCall);

            var publisherMock = Substitute.For<IPublisher>();
            publisherMock.Publish(Arg.Any<object>(), Arg.Any<CancellationToken>())
                     .Returns(Task.CompletedTask);
        }
    }
}