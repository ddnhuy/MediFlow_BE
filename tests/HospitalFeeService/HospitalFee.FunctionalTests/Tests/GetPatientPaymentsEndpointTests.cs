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
using FluentAssertions;
using System.Net.Http.Json;
using HospitalFee.FunctionalTests.Helpers;

namespace HospitalFee.FunctionalTests.Tests
{
    public class GetPatientPaymentsEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetPatientPaymentsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        public Task InitializeAsync() => _factory.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetPatientPayments_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/patients/1/payments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPatientPayments_WithInvalidPatientId_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();

            // Act
            var response = await _client.GetAsync("/patients/0/payments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetPatientPayments_WithExistingPatient_ReturnsOkWithPayments()
        {
            // Arrange
            SetAuthHeader();

            // Seed Data
            var reception = new Reception { PatientId = 1, ServiceTypeId = 1 };
            await SeedEntityAsync(reception);

            var payment = new Payment
            {
                ReceptionId = reception.Id,
                TotalAmount = 150.5m,
                Method = PaymentMethod.CreditCard,
                PaymentType = PaymentType.Receipt,
                Status = PaymentStatus.Completed
            };
            await SeedEntityAsync(payment);

            // Act
            var response = await _client.GetAsync("/patients/1/payments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<GetPatientPaymentsResponse>();
            content.Should().NotBeNull();
            content!.PatientId.Should().Be(1);
            content.Payments.Should().HaveCount(1);
            content.Payments[0].TotalAmount.Should().Be(150.5m);
            content.Payments[0].Method.Should().Be(PaymentMethod.CreditCard);
        }
    }
}
