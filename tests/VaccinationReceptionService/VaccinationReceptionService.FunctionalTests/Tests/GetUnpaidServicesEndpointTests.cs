using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using NSubstitute;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetUnpaidServicesEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;
        private const int TestVaccineId = 1;

        public GetUnpaidServicesEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            var now = DateTime.UtcNow;
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
                    ReceptionDate = now,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create RequestForm with unpaid service if not exists
            //var requestForm = dbContext.RequestForms
            //    .FirstOrDefault(rf => rf.ReceptionId == TestReceptionId);
            //if (requestForm == null)
            //{
            //    requestForm = new RequestForm
            //    {
            //        ReceptionId = TestReceptionId,
            //        RequestNumber = "REQ001",
            //        CreatedAt = now,
            //        CreatedBy = 1,
            //        LastUpdatedAt = now,
            //        LastUpdatedBy = 1
            //    };
            //    dbContext.RequestForms.Add(requestForm);
            //    dbContext.SaveChanges();

                var requestFormService = new ServiceRequestDetail
                {
                    ReceptionId = TestReceptionId,
                    RequestNumber = "REQ001",
                    ServiceId = TestServiceId,
                    Quantity = 1,
                    UnitPrice = 100,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1,
                    InvoiceDate = now
                };
                dbContext.ServiceRequestDetails.Add(requestFormService);
            //}

            // Create ReceptionVaccination with unpaid status if not exists
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.ReceptionId == TestReceptionId && rv.VaccineId == TestVaccineId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    CreatedAt = now,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1,
                    InvoiceDate = now,
                    ScheduledDate = now.AddDays(2),
                    AppointmentDate = now.AddDays(3),
                    IsReadyToUse = false,
                    //IsConfirmed = false
                };

                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetUnpaidServices_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/unpaid-services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUnpaidServices_WithValidData_ReturnsOk()
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

            // Mock HospitalService
            var hospitalServiceMock = _factory.Services.GetRequiredService<IHospitalService>();
            hospitalServiceMock
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
            new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
            {
                Id = TestServiceId,
                ServiceName = "Test Service", // Make sure this matches what your handler expects
                UnitPrice = 100000
            }
                });

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/unpaid-services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UnpaidServicesResponseDTO>();
            result.Should().NotBeNull();

            // Verify services
            result!.Services.Should().NotBeNull();
            result.Services.Should().NotBeEmpty();
            result.Services.First().ServiceId.Should().Be(TestServiceId);

            // Verify vaccinations
            result.Vaccinations.Should().NotBeNull();
            result.Vaccinations.Should().NotBeEmpty();
            result.Vaccinations.First().VaccineId.Should().Be(TestVaccineId);
        }

        [Fact]
        public async Task GetUnpaidServices_WithInvalidReceptionId_ReturnsNotFound()
        {
            var invalidReceptionId = 999;

            var response = await _client.GetAsync($"/receptions/{invalidReceptionId}/unpaid-services");

            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().NotBeNull();
            content!.Should().NotBeNullOrEmpty();
        }
    }
}