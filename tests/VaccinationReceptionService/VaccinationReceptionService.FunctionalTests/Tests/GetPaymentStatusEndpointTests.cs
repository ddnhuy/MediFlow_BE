using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPaymentStatusEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        private const int TestContractId = 91001;
        private const int TestPaymentContractId = 92001;

        public GetPaymentStatusEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                ContractCode = "C-PAY-001",
                ContractNumber = 1,
                ContractName = "Contract for Payment Status",
                CompanyName = "Test Company",
                UnitName = "Test Unit",
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 1000,
                AdvanceAmount = 100,
                ActualAmount = 900,
                Description = "Test contract for payment status",
                Status = ContractStatus.Active,
                ExpectedPatientCount = 10,
                ExpectedVaccineCount = 0,
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();

            dbContext.PaymentContracts.Add(new PaymentContract
            {
                Id = TestPaymentContractId,
                ContractId = TestContractId,
                InvoiceNumber = "INV-PS-001",
                VATInvoiceNumber = "VAT-PS-001",
                InvoiceType = InvoiceType.AdvancePayment,
                TotalAmount = 500,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatus.Pending,
                TaxCode = "TAX-001",
                OrganizationName = "Org A",
                ATMCode = null
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetPaymentStatus_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/payment-status?paymentContractId={TestPaymentContractId}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPaymentStatus_WithoutAnyQueryParams_ReturnsBadRequest()
        {
            // Authorized but missing both ids
            var response = await _client.GetAsync("/payment-status");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetPaymentStatus_WithValidPaymentContractId_ReturnsOkAndStatus()
        {
            SeedData();

            var response = await _client.GetAsync($"/payment-status?paymentContractId={TestPaymentContractId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PaymentStatus>();
            result.Should().Be(PaymentStatus.Pending);
        }

        [Fact]
        public async Task GetPaymentStatus_WithValidPaymentId_ReturnsNotFound()
        {
            SeedData();

            var response = await _client.GetAsync($"/payment-status?paymentId={TestPaymentContractId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
