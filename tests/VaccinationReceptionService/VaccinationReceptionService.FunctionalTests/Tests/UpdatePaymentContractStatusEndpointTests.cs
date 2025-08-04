using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdatePaymentContractStatusEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestContractId = 2001;
        private const int TestPaymentContractId = 3001;

        public UpdatePaymentContractStatusEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.PaymentContracts.RemoveRange(dbContext.PaymentContracts.Where(p => p.Id == TestPaymentContractId));
            dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(c => c.Id == TestContractId));
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
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
                IsSuspended = false
            });
            dbContext.SaveChanges();

            dbContext.PaymentContracts.Add(new PaymentContract
            {
                Id = TestPaymentContractId,
                ContractId = TestContractId,
                InvoiceNumber = "INV-001",
                VATInvoiceNumber = "VAT-001",
                InvoiceType = InvoiceType.AdvancePayment,
                TotalAmount = 500,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatus.Pending,
                TaxCode = "TAXCODE",
                OrganizationName = "Test Org",
                ATMCode = null
            });
            dbContext.SaveChanges();
        }

        private UpdatePaymentContractStatusRequest GetRequest(PaymentStatus status)
        {
            return new UpdatePaymentContractStatusRequest(status);
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = GetRequest(PaymentStatus.Completed);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{TestContractId}/payment-contracts/{TestPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_ContractNotFound_ReturnsNotFound()
        {
            // Arrange
            var notFoundContractId = 99999;
            var request = GetRequest(PaymentStatus.Completed);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{notFoundContractId}/payment-contracts/{TestPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_PaymentContractNotFound_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var notFoundPaymentContractId = 88888;
            var request = GetRequest(PaymentStatus.Completed);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{TestContractId}/payment-contracts/{notFoundPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_ToCompleted_ReturnsOk()
        {
            // Arrange
            SeedData();
            var request = GetRequest(PaymentStatus.Completed);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{TestContractId}/payment-contracts/{TestPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Status updated successfully");
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_ToCancelled_ReturnsOk()
        {
            // Arrange
            SeedData();
            var request = GetRequest(PaymentStatus.Cancelled);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{TestContractId}/payment-contracts/{TestPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Status updated successfully");
        }

        [Fact]
        public async Task UpdatePaymentContractStatus_ToPending_ReturnsOk()
        {
            // Arrange
            SeedData();
            var request = GetRequest(PaymentStatus.Pending);

            // Act
            var response = await _client.PutAsJsonAsync($"/contracts/{TestContractId}/payment-contracts/{TestPaymentContractId}/status", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Status updated successfully");
        }
    }
}
