using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetVaccinationHistoryByPatientIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionId = 1;
        private const int TestReceptionVaccinationId = 1;
        private const int TestVaccinationId = 1;
        private const int TestMedicineId = 1;
        private const int TestDoctorId = 1;

        public GetVaccinationHistoryByPatientIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            ConfigureMocks();
            SeedData();
        }

        private void ConfigureMocks()
        {
            // Setup Patient gRPC mock
            var patientResponse = new PatientDetailModel
            {
                Id = TestPatientId,
                Code = "PAT-001",
                Name = "John Doe",
                Gender = 1, // Male
                PhoneNumber = "123456789",
                AddressDetail = "123 Main St",
                Ward = "Central Ward",
                District = "Downtown",
                Province = "Test Province",
                Dob = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-30))
            };

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(patientResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock?
                .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Setup Medicine Information mock
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = TestMedicineId,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "mRNA",
                MedicineTypeName = "Vaccine",
                Concentration = "30 mg/mL",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Setup ApplicationUser gRPC mock for doctor information
            var doctorResponse = new ApplicationUserDetailModel
            {
                Id = TestDoctorId,
                Name = "Dr. Smith"
            };

            var doctorAsyncUnaryCall = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Metadata>(), null, default)
                .Returns(doctorAsyncUnaryCall);
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
                    PatientId = TestPatientId,
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
                    VaccineId = TestMedicineId,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    //IsConfirmed = false,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    DoctorId = TestDoctorId,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            // Create Vaccination if not exists
            var vaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.Id == TestVaccinationId);
            if (vaccination == null)
            {
                vaccination = new Vaccination
                {
                    Id = TestVaccinationId,
                    PatientId = TestPatientId,
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    MedicineBatchId = 1,
                    BatchNumber = "BATCH-001",
                    MedicineId = TestMedicineId,
                    MedicineName = "COVID-19 Vaccine",
                    VaccinationDate = DateTime.UtcNow,
                    Note = "Test vaccination",
                    DoctorId = TestDoctorId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(vaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithValidData_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.PatientName.Should().Be("John Doe");
            result.Gender.Should().Be("Nam");
            result.PatientCode.Should().Be("PAT-001");

            result.VaccinationHistoryItems.Should().NotBeNull();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetVaccinationHistory_WithNonExistentPatientId_ReturnsEmptyHistory()
        {
            // Act
            var nonExistentPatientId = 9999;
            var response = await _client.GetAsync($"/vaccination/patient/{nonExistentPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();
            result.VaccinationHistoryItems.Should().BeEmpty();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithVaccinationIssues_ReturnsCorrectHistoryItems()
        {
            // Arrange - Create a ReceptionVaccination with issues
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var issueReceptionVaccination = new ReceptionVaccination
            {
                Id = 2,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 2,
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                VaccinationTestDate = DateTime.UtcNow.AddDays(-2),
                HasIssue = true,
                IssueNote = "Adverse reaction reported",
                IssueDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-002",
                UnitPrice = 100.00m,
                DoctorId = TestDoctorId,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.ReceptionVaccinations.Add(issueReceptionVaccination);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithVaccinationIssuesAndExistingVaccinations_ReturnsCorrectHistoryItems()
        {
            // Arrange - Create a ReceptionVaccination with issues AND existing vaccinations (adverse reactions)
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var issueWithVaccinationRV = new ReceptionVaccination
            {
                Id = 3,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 1,
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                VaccinationTestDate = DateTime.UtcNow.AddDays(-3),
                HasIssue = true,
                IssueNote = "Adverse reaction after injection",
                IssueDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-003",
                UnitPrice = 100.00m,
                DoctorId = TestDoctorId,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.ReceptionVaccinations.Add(issueWithVaccinationRV);

            // Create a vaccination for this ReceptionVaccination (vaccine was injected but had issues)
            var vaccinationWithIssue = new Vaccination
            {
                Id = 100,
                PatientId = TestPatientId,
                ReceptionVaccinationId = 3,
                MedicineBatchId = 1,
                BatchNumber = "BATCH-ISSUE",
                MedicineId = TestMedicineId,
                MedicineName = "COVID-19 Vaccine",
                DoseNumber = 1,
                VaccinationDate = DateTime.UtcNow.AddDays(-2), // Vaccinated 2 days ago
                IsConfirmed = true,
                Note = "Vaccination completed but adverse reaction occurred",
                DoctorId = TestDoctorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.Vaccinations.Add(vaccinationWithIssue);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();

            // Should have item with issue but with vaccination details
            var issueWithVaccinationItem = result.VaccinationHistoryItems
                .FirstOrDefault(x => x.ReceptionVaccinationId == 3);

            issueWithVaccinationItem.Should().NotBeNull();
            issueWithVaccinationItem!.HasIssue.Should().BeTrue();
            issueWithVaccinationItem.IssueNote.Should().Be("Adverse reaction after injection");
            issueWithVaccinationItem.IssueDate.Should().NotBeNull();
            issueWithVaccinationItem.VaccinationDate.Should().NotBeNull(); // Has vaccination date
            issueWithVaccinationItem.DoseNumber.Should().Be("Mũi thứ 1"); // Shows actual dose number
            issueWithVaccinationItem.VaccinationConfirmation.Should().BeTrue(); // Was confirmed
            issueWithVaccinationItem.Id.Should().Be(100); // Has vaccination ID
        }

        [Fact]
        public async Task GetVaccinationHistory_WithRejectedVaccineBeforeInjection_ReturnsCorrectHistoryItems()
        {
            // Arrange - Create a ReceptionVaccination with issues but NO vaccinations (rejected before injection)
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var rejectedVaccineRV = new ReceptionVaccination
            {
                Id = 4,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 3, // Planned for 3 doses
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                HasIssue = true,
                IssueNote = "Vaccine rejected due to patient allergy",
                IssueDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-004",
                UnitPrice = 100.00m,
                DoctorId = TestDoctorId,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.ReceptionVaccinations.Add(rejectedVaccineRV);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();

            // Should have 3 items (one for each planned dose) with rejection details
            var rejectedItems = result.VaccinationHistoryItems
                .Where(x => x.ReceptionVaccinationId == 4)
                .ToList();

            rejectedItems.Should().HaveCount(3); // Quantity = 3

            foreach (var item in rejectedItems)
            {
                item.HasIssue.Should().BeTrue();
                item.IssueNote.Should().Be("Vaccine rejected due to patient allergy");
                item.IssueDate.Should().NotBeNull();
                item.VaccinationDate.Should().BeNull(); // No vaccination date since rejected
                item.DoseNumber.Should().Be("N/A"); // No specific dose number
                item.VaccinationConfirmation.Should().BeFalse(); // Not confirmed
                item.Id.Should().BeNull(); // No vaccination ID
                item.MedicineName.Should().Be("COVID-19 Vaccine"); // Uses medicine info from ReceptionVaccination
                item.MedicineTypeName.Should().Be("mRNA"); // Uses medicine info from ReceptionVaccination
            }
        }

        [Fact]
        public async Task GetVaccinationHistory_WithMixedIssueScenarios_ReturnsCorrectHistoryItems()
        {
            // Arrange - Create multiple ReceptionVaccinations with different issue scenarios
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Scenario 1: Rejected before injection (no vaccinations)
            var rejectedRV = new ReceptionVaccination
            {
                Id = 5,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 2,
                HasIssue = true,
                IssueNote = "Rejected before injection",
                IssueDate = DateTime.UtcNow.AddDays(-1),
                VaccinationTestDate = DateTime.UtcNow.AddDays(-2),
                DoctorId = TestDoctorId,
                // ... other required fields
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-005",
                UnitPrice = 100.00m,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            // Scenario 2: Adverse reaction after injection (has vaccinations)
            var adverseReactionRV = new ReceptionVaccination
            {
                Id = 6,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 1,
                HasIssue = true,
                IssueNote = "Adverse reaction after injection",
                IssueDate = DateTime.UtcNow.AddDays(-1),
                VaccinationTestDate = DateTime.UtcNow.AddDays(-3),
                DoctorId = TestDoctorId,
                // ... other required fields
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-006",
                UnitPrice = 100.00m,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.ReceptionVaccinations.Add(rejectedRV);
            dbContext.ReceptionVaccinations.Add(adverseReactionRV);

            // Create vaccination for adverse reaction scenario
            var adverseVaccination = new Vaccination
            {
                Id = 200,
                PatientId = TestPatientId,
                ReceptionVaccinationId = 6,
                MedicineBatchId = 1,
                BatchNumber = "BATCH-ADVERSE",
                MedicineId = TestMedicineId,
                MedicineName = "COVID-19 Vaccine",
                DoseNumber = 1,
                VaccinationDate = DateTime.UtcNow.AddDays(-2),
                IsConfirmed = true,
                Note = "Vaccination completed",
                DoctorId = TestDoctorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.Vaccinations.Add(adverseVaccination);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();

            // Check rejected items (no vaccinations)
            var rejectedItems = result.VaccinationHistoryItems
                .Where(x => x.ReceptionVaccinationId == 5)
                .ToList();

            rejectedItems.Should().HaveCount(2); // Quantity = 2
            rejectedItems.All(x => x.HasIssue && x.VaccinationDate == null && x.DoseNumber == "N/A").Should().BeTrue();

            // Check adverse reaction items (has vaccinations)
            var adverseItems = result.VaccinationHistoryItems
                .Where(x => x.ReceptionVaccinationId == 6)
                .ToList();

            adverseItems.Should().HaveCount(1); // Quantity = 1
            adverseItems.All(x => x.HasIssue && x.VaccinationDate != null && x.DoseNumber == "Mũi thứ 1").Should().BeTrue();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithIssueButNoDoctor_ReturnsCorrectDoctorName()
        {
            // Arrange - Create a ReceptionVaccination with issues but no doctor assigned
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var noDoctorIssueRV = new ReceptionVaccination
            {
                Id = 7,
                ReceptionId = TestReceptionId,
                VaccineId = TestMedicineId,
                Quantity = 1,
                HasIssue = true,
                IssueNote = "Issue with no doctor",
                IssueDate = DateTime.UtcNow,
                VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                DoctorId = null, // No doctor assigned
                // ... other required fields
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                RequestNumber = "REQ-007",
                UnitPrice = 100.00m,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.ReceptionVaccinations.Add(noDoctorIssueRV);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();

            // Should have item with empty doctor name
            var noDoctorIssueItem = result.VaccinationHistoryItems
                .FirstOrDefault(x => x.ReceptionVaccinationId == 7);

            noDoctorIssueItem.Should().NotBeNull();
            noDoctorIssueItem!.HasIssue.Should().BeTrue();
            noDoctorIssueItem.DoctorName.Should().BeEmpty(); // No doctor assigned
            noDoctorIssueItem.IssueNote.Should().Be("Issue with no doctor");
        }

        // ... existing code ...
    }
}