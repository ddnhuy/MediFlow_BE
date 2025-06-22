using BuildingBlocks.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.HospitalFeeEndpoints;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;
using HospitalFee.FunctionalTests.Abstractions;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using HospitalFee.FunctionalTests.Helpers;

namespace HospitalFee.FunctionalTests.Tests
{
    public class RefundPaymentEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly FunctionalTestWebAppFactory _factory;
        private readonly string _testToken;

        public RefundPaymentEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
        public async Task RefundPayment_WithValidPaidItems_ReturnsCreated()
        {
            // Arrange
            SetAuthHeader();

            var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
            await SeedEntityAsync(reception);
            var requestForm = new RequestForm { ReceptionId = reception.Id, RequestNumber = "REQ-001" };
            await SeedEntityAsync(requestForm);

            var paidService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 101, PaymentStatus = PaymentStatusForItem.Paid, UnitPrice = 100, Quantity = 1, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) };
            await SeedEntityAsync(paidService);

            var originalPayment = new Payment { ReceptionId = reception.Id, Method = PaymentMethod.Cash, TotalAmount = 100, Status = PaymentStatus.Completed, PaymentDetails = new List<PaymentDetail> { new() { ServiceRequestDetailId = paidService.Id, Amount = 100 } } };
            await SeedEntityAsync(originalPayment);

            var request = new RefundPaymentRequest(
                Method: PaymentMethod.Cash,
                Note: "Customer requested refund.",
                RefundedReceptionVaccinationIds: new List<int>(),
                RefundedServiceRequestDetailIds: new List<int> { paidService.Id }
            );

            // Act
            var response = await _client.PostAsJsonAsync($"/receptions/{reception.Id}/payments/{originalPayment.Id}/refund", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var refundPayment = await _dbContext.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentType == PaymentType.Refund);
            refundPayment.Should().NotBeNull();
            refundPayment!.TotalAmount.Should().Be(-100m);
            var refundedService = await _dbContext.ServiceRequestDetails.AsNoTracking().FirstAsync(s => s.Id == paidService.Id);
            refundedService.PaymentStatus.Should().Be(PaymentStatusForItem.Refunded);
        }

        //[Fact]
        //public async Task RefundPayment_ForUnpaidItem_ReturnsBadRequest()
        //{
        //    // Arrange
        //    SetAuthHeader();

        //    var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
        //    await SeedEntityAsync(reception);
        //    var originalPayment = new Payment { ReceptionId = reception.Id, Method = PaymentMethod.Cash, Status = PaymentStatus.Completed };
        //    await SeedEntityAsync(originalPayment);

        //    var requestForm = new RequestForm { ReceptionId = reception.Id, RequestNumber = "REQ-002" };
        //    await SeedEntityAsync(requestForm);

        //    var unpaidService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 102, PaymentStatus = PaymentStatusForItem.NotPaid, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) };
        //    await SeedEntityAsync(unpaidService);

        //    var request = new RefundPaymentRequest(
        //        Method: PaymentMethod.Cash,
        //        Note: "Attempting invalid refund.",
        //        RefundedReceptionVaccinationIds: new List<int>(),
        //        RefundedServiceRequestDetailIds: new List<int> { unpaidService.Id }
        //    );

        //    // Act
        //    var response = await _client.PostAsJsonAsync($"/receptions/{reception.Id}/payments/{originalPayment.Id}/refund", request);

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //    var content = await response.Content.ReadAsStringAsync();
        //    content.Should().Contain(ExceptionKey.REFUNDED_ITEMS_NOT_PAID_OR_INVALID.ToString());
        //}

        [Fact]
        public async Task RefundPayment_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null; // No token
            var request = new RefundPaymentRequest(PaymentMethod.Cash, null, new List<int>(), new List<int>());

            // Act
            var response = await _client.PostAsJsonAsync("/receptions/1/payments/1/refund", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
