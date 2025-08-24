using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Strings;
using System.Text.Json;
using VaccinationReception.Application.Vaccinations.Queries.GetPendingVaccinationsTodayQuery;
using VaccinationReception.Domain.Models;
using VaccinationReception.Domain.Enums;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPendingVaccinationsTodayEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 100;
        private const int TestInvalidReceptionId = 999;
        private const int TestPatientId = 1;
        private const int TestVaccineId = 1;
        private const int TestServiceTypeId = 1;

        public GetPendingVaccinationsTodayEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedTestData();
            SetupInventoryServiceMock();
        }

        private void SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clean existing data
            dbContext.ReceptionVaccinations.RemoveRange(dbContext.ReceptionVaccinations.Where(rv => rv.ReceptionId == TestReceptionId));
            dbContext.Vaccinations.RemoveRange(dbContext.Vaccinations);
            dbContext.Receptions.RemoveRange(dbContext.Receptions.Where(r => r.Id == TestReceptionId));
            dbContext.ServiceTypes.RemoveRange(dbContext.ServiceTypes.Where(st => st.Id == TestServiceTypeId));

            // Create ServiceType
            var serviceType = new VaccinationReception.Domain.Models.ServiceType
            {
                Id = TestServiceTypeId,
                Name = "Vaccination Service",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.ServiceTypes.Add(serviceType);

            // Create Reception
            var reception = new Reception
            {
                Id = TestReceptionId,
                ServiceTypeId = TestServiceTypeId,
                PatientId = TestPatientId,
                ReceptionDate = DateTime.UtcNow,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Receptions.Add(reception);

            // Create ReceptionVaccination scheduled for today
            var today = DateTime.UtcNow.Date;
            var receptionVaccination = new ReceptionVaccination
            {
                Id = 1,
                ReceptionId = TestReceptionId,
                VaccineId = TestVaccineId,
                Quantity = 3, // Total 3 doses needed
                ScheduledDate = today.AddHours(10), // Today at 10 AM
                TestResultEntry = "negative",
                HasIssue = false,
                IsCancelled = false,
                IsReadyToUse = true,
                RequestNumber = "REQ-001", // Required field
                UnitPrice = 150000m, // Required field
                PaymentStatus = PaymentStatusForItem.NotPaid, // Required field
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.ReceptionVaccinations.Add(receptionVaccination);

            // Create 1 completed vaccination (so 2 doses are still pending)
            var completedVaccination = new Vaccination
            {
                Id = 1,
                PatientId = TestPatientId,
                ReceptionVaccinationId = 1,
                MedicineBatchId = 1,
                BatchNumber = "BATCH-001",
                MedicineId = TestVaccineId,
                MedicineName = "COVID-19 Vaccine",
                IsConfirmed = true,
                DoctorId = 1,
                DoseNumber = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Vaccinations.Add(completedVaccination);

            dbContext.SaveChanges();
        }

        private void SetupInventoryServiceMock()
        {
            var medicineInfo = new GetMedicineInformationResponse
            {
                MedicineId = TestVaccineId,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "COVID-19",
                RouteOfAdministration = "IM",
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);
        }

        [Fact]
        public async Task GetPendingVaccinationsToday_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestReceptionId}/pending-vaccinations-today");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPendingVaccinationsToday_WithValidReceptionId_ReturnsOkWithPendingVaccinations()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestReceptionId}/pending-vaccinations-today");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPendingVaccinationsTodayResult>();

        }

        [Fact]
        public async Task GetPendingVaccinationsToday_WithInvalidReceptionId_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestInvalidReceptionId}/pending-vaccinations-today");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorString = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ProblemDetails>(errorString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            error.Should().NotBeNull();
            error!.Detail.Should().Be(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID.ToString());
        }

        [Fact]
        public async Task GetPendingVaccinationsToday_WithNoPendingVaccinations_ReturnsOkWithEmptyResult()
        {
            // Arrange - Create a reception without any vaccination scheduled for today
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var emptyReceptionId = 200;
            var emptyReception = new Reception
            {
                Id = emptyReceptionId,
                ServiceTypeId = TestServiceTypeId,
                PatientId = TestPatientId + 1,
                ReceptionDate = DateTime.UtcNow,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Receptions.Add(emptyReception);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{emptyReceptionId}/pending-vaccinations-today");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPendingVaccinationsTodayResult>();

            result.Should().NotBeNull();
            result!.TotalPendingDoses.Should().Be(0);
            result.PendingVaccinations.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPendingVaccinationsToday_WithCancelledReception_ReturnsBadRequest()
        {
            // Arrange - Create a cancelled reception
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cancelledReceptionId = 300;
            var cancelledReception = new Reception
            {
                Id = cancelledReceptionId,
                ServiceTypeId = TestServiceTypeId,
                PatientId = TestPatientId + 2,
                ReceptionDate = DateTime.UtcNow,
                IsCancelled = true, // This reception is cancelled
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Receptions.Add(cancelledReception);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{cancelledReceptionId}/pending-vaccinations-today");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorString = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ProblemDetails>(errorString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            error.Should().NotBeNull();
            error!.Detail.Should().Be(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID.ToString());
        }
    }
}