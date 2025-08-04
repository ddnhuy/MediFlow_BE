using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using FluentAssertions;
using HospitalFee.FunctionalTests.Abstractions;
using HospitalFee.FunctionalTests.DataTest;
using HospitalFee.FunctionalTests.Helpers;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.HospitalFeeEndpoints;
using VaccinationReception.Domain.DTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace HospitalFee.FunctionalTests.Tests
{
    public class GetPaymentDetailsEndpointTests : BaseFunctionalTest
    {
        private string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        public GetPaymentDetailsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory) 
        {
            _testToken = TokenHelper.GenerateTestToken();
            _factory = factory;
        }
        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetPaymentDetails_WithValidPaymentId_ReturnsOk()
        {
            SetAuthHeader();

            // Arrange
            var paymentId = 123;
            var serviceId = 10;
            var vaccineId = 20;

            var serviceType = new ServiceType
            {
                Id = 4,
                Name = "General Checkup",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            var reception = new Reception
            {
                Id = 1,
                PatientId = 1,
                ServiceTypeId = serviceType.Id,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            var payment = new Payment
            {
                Id = paymentId,
                ReceptionId = 1,
                TotalAmount = 100,
                Method = PaymentMethod.Cash,
                Note = "Test payment",
                ATMTransactionCode = null,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-001",
                Status = PaymentStatus.Pending,
                OriginalPaymentId = null,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsCancelled = false,
                PaymentDetails = new List<PaymentDetail>
                {
                    new PaymentDetail
                    {
                        Id = 1,
                        PaymentId = paymentId,
                        ReceptionVaccinationId = null,
                        ServiceRequestDetailId = 1,
                        Amount = 50,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        IsCancelled = false,
                    },
                    new PaymentDetail
                    {
                        Id = 2,
                        PaymentId = paymentId,
                        ReceptionVaccinationId = 1,
                        ServiceRequestDetailId = null,
                        Amount = 50,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        IsCancelled = false,
                    }
                }
            };

            var serviceRequestDetail = new ServiceRequestDetail
            {
                Id = 1,
                ServiceId = serviceId,
                ReceptionId = 1,
                RequestNumber = "REQ-001"
            };

            var receptionVaccination = new ReceptionVaccination
            {
                Id = 1,
                ReceptionId = 1,
                VaccineId = vaccineId,
                RequestNumber = "REQ-VACC-001"
            };

            var paymentDetailsList = payment.PaymentDetails.ToList();
            paymentDetailsList[0].ServiceRequestDetail = serviceRequestDetail;
            paymentDetailsList[1].ReceptionVaccination = receptionVaccination;

            await SeedEntityAsync(serviceType);
            await SeedEntityAsync(reception);
           // await SeedEntityAsync(requestForm);
            await SeedEntityAsync(serviceRequestDetail);
            await SeedEntityAsync(receptionVaccination);
            await SeedEntityAsync(payment);

            // Mock IHospitalService
            var serviceList = new List<ServiceDTO>
            {
                new ServiceDTO
                {
                    Id = serviceId,
                    ServiceCode = "SRV-001",
                    ServiceName = "Blood Test"
                }
            };

            _factory.HospitalServiceMock!
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(serviceList);

            // Mock IInventoryService
            var vaccineList = new List<GetMedicineInformationResponse>
            {
                new GetMedicineInformationResponse
                {
                    MedicineId = vaccineId,
                    MedicineName = "COVID-19 Vaccine",
                    VaccineTypeName = "COVID-19",
                    MedicineTypeName = "Vaccine",
                    IsSuccess = true
                }
            };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(vaccineList);

            // Act
            var response = await _client.GetAsync($"/payments/{paymentId}/details");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetPaymentDetailsResponse>();
            result.Should().NotBeNull();

            result!.Payment.Should().NotBeNull();
            result.Payment.Id.Should().Be(paymentId);

            result.PaymentDetails.Should().NotBeNullOrEmpty();
            result.PaymentDetails.Should().Contain(pd =>
                (!string.IsNullOrEmpty(pd.ServiceCode) && !string.IsNullOrEmpty(pd.ServiceName))
            );
        }

        [Fact]
        public async Task GetPaymentDetails_WithoutToken_ReturnsUnauthorized()
        {
            
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/payments/1/details");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPaymentDetails_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var invalidPaymentId = 0;

            // Act
            var response = await _client.GetAsync($"/payments/{invalidPaymentId}/details");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
