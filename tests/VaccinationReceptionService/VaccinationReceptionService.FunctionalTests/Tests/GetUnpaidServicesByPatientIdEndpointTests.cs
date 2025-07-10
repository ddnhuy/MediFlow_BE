using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetUnpaidServicesByPatientIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionId = 100;
        private const int TestServiceId = 1;
        private const int TestVaccineId = 1;

        public GetUnpaidServicesByPatientIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            var now = DateTime.UtcNow;

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Reception
            if (!dbContext.Receptions.Any(r => r.Id == TestReceptionId))
            {
                var reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = TestPatientId,
                    ReceptionDate = now,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create RequestForm & Service
            var requestForm = dbContext.RequestForms.FirstOrDefault(rf => rf.ReceptionId == TestReceptionId);
            if (requestForm == null)
            {
                requestForm = new RequestForm
                {
                    ReceptionId = TestReceptionId,
                    RequestNumber = "REQ-PATIENT",
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1
                };
                dbContext.RequestForms.Add(requestForm);
                dbContext.SaveChanges();

                dbContext.ServiceRequestDetails.Add(new ServiceRequestDetail
                {
                    RequestFormId = requestForm.Id,
                    ServiceId = TestServiceId,
                    Quantity = 2,
                    UnitPrice = 200,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1,
                    InvoiceDate = now
                });
            }

            // Add unpaid ReceptionVaccination
            if (!dbContext.ReceptionVaccinations.Any(rv => rv.ReceptionId == TestReceptionId))
            {
                dbContext.ReceptionVaccinations.Add(new ReceptionVaccination
                {
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    RequestNumber = "REQ-PATIENT",
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1,
                    InvoiceDate = now
                });
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetUnpaidServicesByPatientId_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/patients/{TestPatientId}/unpaid-services");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUnpaidServicesByPatientId_WithValidData_ReturnsOk()
        {
            var hospitalServiceMock = _factory.Services.GetRequiredService<IHospitalService>();
            hospitalServiceMock
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new()
                    {
                        Id = TestServiceId,
                        ServiceName = "General Consultation",
                        UnitPrice = 200
                    }
                });

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<GetMedicineInformationResponse>
                {
                    new()
                    {
                        MedicineId = TestVaccineId,
                        MedicineName = "Hepatitis B Vaccine",
                        MedicineTypeName = "Vaccine",
                        VaccineTypeName = "Hepatitis B",
                        IsSuccess = true
                    }
                });

            var response = await _client.GetAsync($"/patients/{TestPatientId}/unpaid-services");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<UnpaidServicesByPatientResponseDTO>();

            result.Should().NotBeNull();
            result!.Services.Should().NotBeEmpty();
            result.Services.First().ServiceId.Should().Be(TestServiceId);

            result.Vaccinations.Should().NotBeEmpty();
            result.Vaccinations.First().VaccineId.Should().Be(TestVaccineId);

            result!.ReceptionId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetUnpaidServicesByPatientId_WithInvalidPatientId_ReturnsBadRequest()
        {
            var response = await _client.GetAsync($"/patients/0/unpaid-services");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetUnpaidServicesByPatientId_WithNotFoundData_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"/patients/9999/unpaid-services");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
        }
    }
}
