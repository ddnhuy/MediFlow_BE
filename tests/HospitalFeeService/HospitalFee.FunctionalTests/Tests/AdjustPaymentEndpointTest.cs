using BuildingBlocks.Strings;
using FluentAssertions;
using HospitalFee.FunctionalTests.Abstractions;
using HospitalFee.FunctionalTests.DataTest;
using HospitalFee.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore;
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
    public class AdjustPaymentEndpointTest : BaseFunctionalTest
    {
        private string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public AdjustPaymentEndpointTest(FunctionalTestWebAppFactory factory) : base(factory) 
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
        }
        public Task InitializeAsync() => _factory.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task AdjustPayment_WithValidData_ReturnsCreatedAndAdjustsPaymentCorrectly()
        {
            // Arrange
            SetAuthHeader();

            // --- Let the database generate all IDs ---
            var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
            await SeedEntityAsync(reception); // Save to get the generated ID

            var requestForm = new RequestForm { ReceptionId = reception.Id, RequestNumber = "REQ-001" };
            await SeedEntityAsync(requestForm);

            var paidService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 101, PaymentStatus = PaymentStatusForItem.Paid, UnitPrice = 50, Quantity = 1, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Unspecified) };
            var paidVaccination = new ReceptionVaccination { ReceptionId = reception.Id, VaccineId = 201, PaymentStatus = PaymentStatusForItem.Paid, UnitPrice = 100, Quantity = 2, RequestNumber = "RV-PAID-001" };
            await SeedEntityAsync(paidService);
            await SeedEntityAsync(paidVaccination);

            var originalPayment = new Payment
            {
                ReceptionId = reception.Id,
                TotalAmount = 250,
                Status = PaymentStatus.Completed,
                Method = PaymentMethod.Cash,
            };
            await SeedEntityAsync(originalPayment); // Save to get the payment's generated ID

            // Create payment details after the payment has an ID
            var paymentDetails = new List<PaymentDetail>
            {
                new() { PaymentId = originalPayment.Id, ServiceRequestDetailId = paidService.Id, Amount = 50 },
                new() { PaymentId = originalPayment.Id, ReceptionVaccinationId = paidVaccination.Id, Amount = 200 }
            };
            await SeedEntitiesAsync(paymentDetails);


            var newService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 102, PaymentStatus = PaymentStatusForItem.NotPaid, UnitPrice = 30, Quantity = 1, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Unspecified) };
            var newVaccination = new ReceptionVaccination { ReceptionId = reception.Id, VaccineId = 202, PaymentStatus = PaymentStatusForItem.NotPaid, UnitPrice = 120, Quantity = 1, RequestNumber = "RV-NEW-001" };
            await SeedEntityAsync(newService);
            await SeedEntityAsync(newVaccination);

            var request = new AdjustPaymentRequest(
                PaymentMethod.Cash,
                "Adjusting payment",
                new List<int> { paidVaccination.Id },
                new List<int> { paidService.Id },
                new List<int> { newVaccination.Id },
                new List<int> { newService.Id }
            );

            // Act
            var response = await _client.PostAsJsonAsync($"/receptions/{reception.Id}/payments/{originalPayment.Id}/adjust", request);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var adjustmentPayment = await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentType == PaymentType.Adjustment);
            adjustmentPayment.Should().NotBeNull();
            adjustmentPayment!.TotalAmount.Should().Be(-100m);

            var updatedOriginalPayment = await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == originalPayment.Id);
            updatedOriginalPayment.Should().NotBeNull();
            updatedOriginalPayment!.Status.Should().Be(PaymentStatus.Adjusted);

            var cancelledService = await _dbContext.ServiceRequestDetails.AsNoTracking().FirstOrDefaultAsync(s => s.Id == paidService.Id);
            cancelledService.Should().NotBeNull();
            cancelledService!.PaymentStatus.Should().Be(PaymentStatusForItem.AdjustedOut);

            var addedService = await _dbContext.ServiceRequestDetails.AsNoTracking().FirstOrDefaultAsync(s => s.Id == newService.Id);
            addedService.Should().NotBeNull();
            addedService!.PaymentStatus.Should().Be(PaymentStatusForItem.Paid);
        }

        [Fact]
        public async Task AdjustPayment_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new AdjustPaymentRequest(PaymentMethod.Cash, null, new(), new(), new(), new());

            // Act
            var response = await _client.PostAsJsonAsync("/receptions/1/payments/1/adjust", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AdjustPayment_WithNonExistentOriginalPayment_ReturnsNotFound()
        {
            // Arrange
            SetAuthHeader();
            var request = new AdjustPaymentRequest(PaymentMethod.Cash, null, new(), new(), new(), new());

            // Act
            var response = await _client.PostAsJsonAsync("/receptions/1/payments/999/adjust", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AdjustPayment_TryingToCancelUnpaidItem_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var (reception, payment, unpaidService) = await TestDataFactory.SeedScenarioForCancellingUnpaidItemAsync(_dbContext);

            var request = new AdjustPaymentRequest(
                PaymentMethod.Cash,
                null,
                CancelledReceptionVaccinationIds: new(),
                CancelledServiceRequestDetailIds: new List<int> { 12 },
                NewReceptionVaccinationIds: new(),
                NewServiceRequestDetailIds: new()
            );

            // Act
            var response = await _client.PostAsJsonAsync("/receptions/2/payments/2/adjust", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(ExceptionKey.CANCEL_ITEMS_NOT_PAID_OR_INVALID.ToString());
        }
    }
}
