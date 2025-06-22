using BuildingBlocks.Strings;
using FluentAssertions;
using HospitalFee.FunctionalTests.Abstractions;
using HospitalFee.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.HospitalFeeEndpoints;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace HospitalFee.FunctionalTests.Tests
{
    public class CreatePaymentEndpointTest : BaseFunctionalTest
    {
        private string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public CreatePaymentEndpointTest(FunctionalTestWebAppFactory factory) : base(factory)
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
        public async Task CreatePayment_WithValidUnpaidItems_ReturnsCreated()
        {
            // Arrange
            SetAuthHeader();
            var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
            await SeedEntityAsync(reception);
            var requestForm = new RequestForm { ReceptionId = reception.Id, RequestNumber = "REQ-001" };
            await SeedEntityAsync(requestForm);

            var unpaidService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 101, PaymentStatus = PaymentStatusForItem.NotPaid, UnitPrice = 50, Quantity = 2, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) }; // Total 100
            var unpaidVaccination = new ReceptionVaccination { ReceptionId = reception.Id, VaccineId = 201, PaymentStatus = PaymentStatusForItem.NotPaid, UnitPrice = 150, Quantity = 1, RequestNumber = "RV-001" }; // Total 150
            await SeedEntityAsync(unpaidService);
            await SeedEntityAsync(unpaidVaccination);

            var request = new CreatePaymentRequest(
                Method: PaymentMethod.CreditCard,
                Note: "First payment",
                InvoiceNumber: "INV-001",
                OfficialInvoiceNumber: null,
                ReceptionVaccinationIds: new List<int> { unpaidVaccination.Id },
                ServiceRequestDetailIds: new List<int> { unpaidService.Id }
            );

            // Act
            var response = await _client.PostAsJsonAsync($"/receptions/{reception.Id}/payments", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPayment = await _dbContext.Payments.AsNoTracking().FirstAsync();
            createdPayment.TotalAmount.Should().Be(250m);
            createdPayment.Method.Should().Be(PaymentMethod.CreditCard);
            createdPayment.PaymentType.Should().Be(PaymentType.Receipt);
        }

        [Fact]
        public async Task CreatePayment_ForAlreadyPaidItem_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
            await SeedEntityAsync(reception);

            var requestForm = new RequestForm { ReceptionId = reception.Id, RequestNumber = "REQ-PAID-002" };
            await SeedEntityAsync(requestForm);

            var paidService = new ServiceRequestDetail { RequestFormId = requestForm.Id, ServiceId = 101, PaymentStatus = PaymentStatusForItem.Paid, UnitPrice = 50, Quantity = 1, InvoiceDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) };
            await SeedEntityAsync(paidService);

            var request = new CreatePaymentRequest(
                Method: PaymentMethod.Cash,
                Note: null,
                InvoiceNumber: null,
                OfficialInvoiceNumber: null,
                ReceptionVaccinationIds: new List<int>(),
                ServiceRequestDetailIds: new List<int> { paidService.Id }
            );

            // Act
            var response = await _client.PostAsJsonAsync($"/receptions/{reception.Id}/payments", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(ExceptionKey.ONE_OR_MORE_ITEMS_ALREADY_PAID_OR_INVALID.ToString());
        }

        [Fact]
        public async Task CreatePayment_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null; // No token
            var request = new CreatePaymentRequest(PaymentMethod.Cash, null, null, null, new List<int>(), new List<int>());

            // Act
            var response = await _client.PostAsJsonAsync("/receptions/1/payments", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
