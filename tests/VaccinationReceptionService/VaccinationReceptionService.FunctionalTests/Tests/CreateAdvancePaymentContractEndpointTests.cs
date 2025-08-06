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
    public class CreateAdvancePaymentContractEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestContractId = 1001;

        public CreateAdvancePaymentContractEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedContract(ContractStatus status = ContractStatus.Active, decimal contractValue = 1000)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                Status = status,
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();
        }

        private CreateAdvancePaymentContractRequest GetValidRequest(decimal advanceAmount = 500)
        {
            return new CreateAdvancePaymentContractRequest(
                AdvanceAmount: advanceAmount,
                PaymentMethod: PaymentMethod.Cash,
                VATInvoiceNumber: "VAT123",
                TaxCode: "TAXCODE",
                OrganizationName: "Test Org"
            );
        }

        [Fact]
        public async Task CreateAdvancePaymentContract_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/advance-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateAdvancePaymentContract_ContractNotFound_ReturnsNotFound()
        {
            // Arrange
            var notFoundContractId = 99999;
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{notFoundContractId}/advance-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateAdvancePaymentContract_ContractNotActive_ReturnsBadRequest()
        {
            // Arrange
            SeedContract(status: ContractStatus.Draft);
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/advance-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateAdvancePaymentContract_AdvanceAmountExceedsContractValue_ReturnsBadRequest()
        {
            // Arrange
            SeedContract(status: ContractStatus.Active, contractValue: 1000);
            var request = GetValidRequest(advanceAmount: 2000);

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/advance-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateAdvancePaymentContract_WithValidData_ReturnsCreatedAndResponse()
        {
            SeedContract(status: ContractStatus.Active);
            var request = GetValidRequest();

            // Act
            var response = await _client.PostAsJsonAsync($"/contracts/{TestContractId}/advance-payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
