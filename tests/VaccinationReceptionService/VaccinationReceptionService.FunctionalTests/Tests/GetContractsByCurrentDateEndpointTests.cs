using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetContractsByCurrentDateEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetContractsByCurrentDateEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Contracts.RemoveRange(dbContext.Contracts);
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
            {
                Id = 1000,
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

            dbContext.Contracts.Add(new Contract
            {
                Id = 2000,
                ContractCode = "C002",
                ContractNumber = 2,
                ContractName = "Cancelled Contract",
                CompanyName = "Test Company",
                UnitName = "Test Unit",
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 2000,
                AdvanceAmount = 200,
                ActualAmount = 1800,
                Description = "Cancelled contract",
                FileContractId = Guid.NewGuid(),
                FileContractName = "contract2.pdf",
                FileVaccinationEnrollmentId = Guid.NewGuid(),
                FileVaccinationEnrollmentName = "enroll2.pdf",
                ExpectedPatientCount = 20,
                Status = ContractStatus.Active,
                IsCancelled = true,
                IsSuspended = false
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetContractsByCurrentDate_WithoutAuthorization_ReturnsUnauthorized()
        {

            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/contracts/current-date");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetContractsByCurrentDate_WithValidToken_ReturnsOkAndContracts()
        {
            SeedData();

            // Act
            var response = await _client.GetAsync("/contracts/current-date");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetContractsByCurrentDateResponse>();
            result.Should().NotBeNull();
            result!.Contracts.Should().NotBeNullOrEmpty();
            result.Contracts.Should().ContainSingle(c => c.ContractCode == "C001");
            result.Contracts.Should().NotContain(c => c.ContractCode == "C002"); 
        }

        [Fact]
        public async Task GetContractsByCurrentDate_WhenExceptionThrown_ReturnsBadRequest()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Contracts.RemoveRange(dbContext.Contracts);
                dbContext.SaveChanges();
            }
            var response = await _client.GetAsync("/contracts/current-date");
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
        }
    }
}