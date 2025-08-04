using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using BuildingBlocks.Strings.Enums;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CreateFinalInvoicePaymentContractEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestContractId = 4001;
        private const int TestReceptionId = 5001;
        private const int TestServiceDetailId = 6001;
        private const int TestVaccineId = 7001;
        private const int TestServiceId = 8001;

        public CreateFinalInvoicePaymentContractEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.ServiceRequestDetails.RemoveRange(dbContext.ServiceRequestDetails);
            dbContext.ReceptionVaccinations.RemoveRange(dbContext.ReceptionVaccinations);
            dbContext.Receptions.RemoveRange(dbContext.Receptions.Where(r => r.Id == TestReceptionId));
            dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(c => c.Id == TestContractId));
            dbContext.SaveChanges();

            var contract = new Contract
            {
                Id = TestContractId,
                ContractCode = "C001",
                ContractNumber = 1,
                ContractName = "Test Contract 1",
                CompanyName = "Test Company",
                UnitName = "Test Unit",
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 1000,
                AdvanceAmount = 100,
                ActualAmount = 900,
                Description = "Test contract for today",
                FileContractId = Guid.NewGuid(),
                FileContractName = "contract.pdf",
                FileVaccinationEnrollmentId = Guid.NewGuid(),
                FileVaccinationEnrollmentName = "enroll.pdf",
                ExpectedPatientCount = 10,
                Status = ContractStatus.Active,
                IsCancelled = false,
                IsSuspended = false,
                ServiceDetails = new List<ContractServiceDetail>
                {
                    new ContractServiceDetail
                    {
                        Id = TestServiceDetailId,
                        ContractId = TestContractId,
                        ServiceId = TestServiceId,
                        VaccineId = null,
                        UnitPrice = 100,
                        Quantity = 2,
                        TotalAmount = 200
                    }
                }
            };
            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            dbContext.Receptions.Add(new Reception
            {
                Id = TestReceptionId,
                ContractId = TestContractId,
                ServiceTypeId = 1,
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            });
            dbContext.SaveChanges();

            dbContext.ReceptionVaccinations.Add(new ReceptionVaccination
            {
                Id = 1,
                ReceptionId = TestReceptionId,
                VaccineId = TestVaccineId,
                Quantity = 1,
                UnitPrice = 100,
                IsReadyToUse = true,
                ScheduledDate = DateTime.UtcNow,
                InvoiceDate = DateTime.UtcNow,
                AppointmentDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatusForItem.Paid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1,
                RequestNumber = "REQ-001"
            });
            dbContext.SaveChanges();

            dbContext.ServiceRequestDetails.Add(new ServiceRequestDetail
            {
                Id = 1,
                RequestNumber = "abc123",
                ReceptionId = TestReceptionId,
                ServiceId = TestServiceId,
                Quantity = 2,
                UnitPrice = 100,
                PaymentStatus = PaymentStatusForItem.Paid
            });
            dbContext.SaveChanges();
        }

        private CreateFinalInvoicePaymentContractRequest GetValidRequest()
        {
            return new CreateFinalInvoicePaymentContractRequest(
                PaymentMethod: PaymentMethod.Cash,
                VATInvoiceNumber: "VAT999",
                TaxCode: "TAXCODE",
                OrganizationName: "Test Org"
            );
        }

        [Fact]
        public async Task CreateFinalInvoicePaymentContract_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/final-invoice-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateFinalInvoicePaymentContract_ContractNotFound_ReturnsNotFound()
        {
            // Arrange
            var notFoundContractId = 99999;
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{notFoundContractId}/final-invoice-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateFinalInvoicePaymentContract_NoReceptionForContract_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(c => c.Id == TestContractId));
                dbContext.SaveChanges();

                dbContext.Contracts.Add(new Contract
                {
                    Id = 1000,
                    ContractCode = "C0012",
                    ContractNumber = 1,
                    ContractName = "Test Contract 1",
                    CompanyName = "Test Company",
                    UnitName = "Test Unit",
                    ContractDate = DateTime.UtcNow,
                    ExpectedDate = DateTime.UtcNow,
                    ContractValue = 1000,
                    AdvanceAmount = 100,
                    ActualAmount = 900,
                    Description = "Test contract for today",
                    FileContractId = Guid.NewGuid(),
                    FileContractName = "contract.pdf",
                    FileVaccinationEnrollmentId = Guid.NewGuid(),
                    FileVaccinationEnrollmentName = "enroll.pdf",
                    ExpectedPatientCount = 10,
                    Status = ContractStatus.Active,
                    IsCancelled = false,
                    IsSuspended = false,
                    ServiceDetails = new List<ContractServiceDetail>()
                });
                dbContext.SaveChanges();
            }

            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/final-invoice-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateFinalInvoicePaymentContract_WithValidData_ReturnsCreatedAndResponse()
        {
            // Arrange
            SeedData();
            var request = GetValidRequest();

            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = 1,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "COVID-19",
                RouteOfAdministration = RouteOfAdministration.IM.ToString(),
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfo2 = new GetMedicineInformationResponse
            {
                MedicineId = 2,
                MedicineName = "Flu Vaccine",
                VaccineTypeName = "Influenza",
                RouteOfAdministration = RouteOfAdministration.IM.ToString(),
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1, medicineInfo2 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            _factory.HospitalServiceMock
                 .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                 .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                 {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "ExamFee",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    },
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 2,
                        ServiceCode = "IM",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    }
                 });
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/final-invoice-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<CreateFinalInvoicePaymentContractResponse>();
            result.Should().NotBeNull();
            result!.ContractId.Should().Be(TestContractId);
            result.PaymentContract.Should().NotBeNull();
            result.Details.Should().NotBeNullOrEmpty();
        }
    }
}
