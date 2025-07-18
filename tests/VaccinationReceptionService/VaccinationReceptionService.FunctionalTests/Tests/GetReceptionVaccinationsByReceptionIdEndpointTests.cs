using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using NSubstitute;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetReceptionVaccinationsByReceptionIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;

        public GetReceptionVaccinationsByReceptionIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                .FirstOrDefault(rv => rv.ReceptionId == TestReceptionId && rv.VaccineId == TestVaccineId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    //IsConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1,
                    RequestNumber = "REQ-001"
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetReceptionVaccinationsByReceptionId_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/vaccinations");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }


        [Fact]
        public async Task GetReceptionVaccinationsByReceptionId_WithValidData_ReturnsOk()
        {
            // Arrange
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = 1,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "COVID-19",
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfo2 = new GetMedicineInformationResponse
            {
                MedicineId = 2,
                MedicineName = "Flu Vaccine",
                VaccineTypeName = "Influenza",
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1, medicineInfo2 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/vaccinations");

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetReceptionVaccinationsByReceptionIdResponse>();
            result.Should().NotBeNull();
            result.ReceptionVaccinations.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetReceptionVaccinationsByReceptionId_WithInvalidReceptionId_ReturnsBadRequest()
        {
            // Arrange
            var invalidReceptionId = 0; // Invalid reception ID

            // Act
            var response = await _client.GetAsync($"/receptions/{invalidReceptionId}/vaccinations");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}