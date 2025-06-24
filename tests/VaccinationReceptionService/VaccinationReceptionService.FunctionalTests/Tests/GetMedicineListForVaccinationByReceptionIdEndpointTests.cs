using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId;
using VaccinationReception.Domain.Models;
using VaccinationReception.Domain.Enums;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using MassTransit;
using NSubstitute;
using HumanResource.Grpc;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetMedicineListForVaccinationByReceptionIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestDoctorId = 1;
        private const int TestVaccineId1 = 1;
        private const int TestVaccineId2 = 2;

        public GetMedicineListForVaccinationByReceptionIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create ReceptionVaccination for today (Doctor Prescribed)
            var todayVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == 1);
            if (todayVaccination == null)
            {
                todayVaccination = new ReceptionVaccination
                {
                    Id = 1,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId1,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow.Date, // Today - Doctor Prescribed (UTC)
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.Paid,
                    IsConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    DoctorId = TestDoctorId,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(todayVaccination);
            }

            // Create ReceptionVaccination for future (Customer Warehouse)
            var futureVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == 2);
            if (futureVaccination == null)
            {
                futureVaccination = new ReceptionVaccination
                {
                    Id = 2,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId2,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow.Date.AddDays(7), // Future - Customer Warehouse (UTC)
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.Paid,
                    IsConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-002",
                    UnitPrice = 150.00m,
                    DoctorId = TestDoctorId,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(futureVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetMedicineListForVaccinationByReceptionId_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = TestVaccineId1,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "COVID-19",
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfo2 = new GetMedicineInformationResponse
            {
                MedicineId = TestVaccineId2,
                MedicineName = "Flu Vaccine",
                VaccineTypeName = "Influenza",
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1, medicineInfo2 };

            // Configure the mock from factory
            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Create your response
            var doctorResponse = new ApplicationUserDetailModel
            {
                Id = TestDoctorId,
                Name = "Test Doctor"
            };

            // Create a fake AsyncUnaryCall
            var asyncUnaryCall = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _factory.ApplicationUserProtoMock
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _factory.InventoryServiceMock.Received(1)
                .GetMedicineInformationAsync(
                    Arg.Is<IEnumerable<int>>(ids => ids.Contains(TestVaccineId1) && ids.Contains(TestVaccineId2)),
                    Arg.Any<CancellationToken>()
                );

            var result = await response.Content.ReadFromJsonAsync<GetMedicineListForVaccinationByReceptionIdResponse>();
            result.Should().NotBeNull();
            result!.DoctorPrescribedVaccines.Should().NotBeNull();
            result.CustomerWarehouseVaccines.Should().NotBeNull();

            // Should have 1 doctor prescribed vaccine (today's date)
            result.DoctorPrescribedVaccines.Should().HaveCount(1);
            var doctorPrescribed = result.DoctorPrescribedVaccines.First();
            doctorPrescribed.MedicineId.Should().Be(TestVaccineId1);
            doctorPrescribed.MedicineName.Should().Be("COVID-19 Vaccine");

            // Should have 1 customer warehouse vaccine (future date)
            result.CustomerWarehouseVaccines.Should().HaveCount(1);
            var customerWarehouse = result.CustomerWarehouseVaccines.First();
            customerWarehouse.MedicineId.Should().Be(TestVaccineId2);
            customerWarehouse.MedicineName.Should().Be("Flu Vaccine");
        }

        [Fact]
        public async Task GetMedicineListForVaccinationByReceptionId_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineListForVaccinationByReceptionId_WithNoReadyToUseVaccines_ReturnsEmptyLists()
        {
            // Arrange
            // Update ReceptionVaccinations to not be ready to use
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var vaccinations = await dbContext.ReceptionVaccinations
                .Where(rv => rv.ReceptionId == TestReceptionId)
                .ToListAsync();

            foreach (var vaccination in vaccinations)
            {
                vaccination.IsReadyToUse = false;
            }
            await dbContext.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineListForVaccinationByReceptionIdResponse>();
            result.Should().NotBeNull();
            result!.DoctorPrescribedVaccines.Should().NotBeNull();
            result.CustomerWarehouseVaccines.Should().NotBeNull();
            result.DoctorPrescribedVaccines.Should().BeEmpty();
            result.CustomerWarehouseVaccines.Should().BeEmpty();

            // Clean up - restore the original state
            foreach (var vaccination in vaccinations)
            {
                vaccination.IsReadyToUse = true;
            }
            await dbContext.SaveChangesAsync();
        }
    }
}