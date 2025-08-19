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
    public class UpdateContractStatusEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        private const int ActiveContractId = 51001;
        private const int CompletedContractId = 52001;

        public UpdateContractStatusEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedActiveContract()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(c => c.Id == ActiveContractId));
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
            {
                Id = ActiveContractId,
                ContractCode = "C-ACT-001",
                ContractNumber = 1,
                ContractName = "Active Contract",
                CompanyName = "Company A",
                UnitName = "Unit A",
                Status = ContractStatus.Active,
                ExpectedPatientCount = 10,
                ExpectedVaccineCount = 0,
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 1000,
                AdvanceAmount = 100,
                ActualAmount = 900,
                Description = "Active contract",
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();
        }

        private void SeedCompletedContract()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(c => c.Id == CompletedContractId));
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
            {
                Id = CompletedContractId,
                ContractCode = "C-CPL-001",
                ContractNumber = 2,
                ContractName = "Completed Contract",
                CompanyName = "Company B",
                UnitName = "Unit B",
                Status = ContractStatus.Completed,
                ExpectedPatientCount = 5,
                ExpectedVaccineCount = 0,
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 2000,
                AdvanceAmount = 200,
                ActualAmount = 1800,
                Description = "Completed contract",
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task UpdateContractStatus_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new UpdateContractStatusRequest(ContractStatus.Active);

            var response = await _client.PutAsJsonAsync($"/contracts/{ActiveContractId}/status", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateContractStatus_WithInvalidCancellation_ReturnsBadRequest()
        {
            SeedCompletedContract();
            var request = new UpdateContractStatusRequest(ContractStatus.Cancelled, "Attempt to cancel completed");

            var response = await _client.PutAsJsonAsync($"/contracts/{CompletedContractId}/status", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateContractStatus_WithValidData_ReturnsOkAndUpdates()
        {
            SeedActiveContract();
            var request = new UpdateContractStatusRequest(ContractStatus.Completed);

            var response = await _client.PutAsJsonAsync($"/contracts/{ActiveContractId}/status", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Contract status updated successfully");

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updated = await dbContext.Contracts.FirstOrDefaultAsync(c => c.Id == ActiveContractId);
            updated.Should().NotBeNull();
            updated!.Status.Should().Be(ContractStatus.Completed);
        }
    }
}
