using BuildingBlocks.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllContractsEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetAllContractsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedContracts()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Contracts.RemoveRange(dbContext.Contracts);
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
            {
                ContractCode = "C001",
                ContractNumber = 1,
                ContractName = "Contract One",
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
                Description = "First contract",
                IsCancelled = false,
                IsSuspended = false
            });

            dbContext.Contracts.Add(new Contract
            {
                ContractCode = "C002",
                ContractNumber = 2,
                ContractName = "Second Contract",
                CompanyName = "Company B",
                UnitName = "Unit B",
                Status = ContractStatus.Active,
                ExpectedPatientCount = 5,
                ExpectedVaccineCount = 0,
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 2000,
                AdvanceAmount = 200,
                ActualAmount = 1800,
                Description = "Second item",
                IsCancelled = false,
                IsSuspended = false
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllContracts_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/contracts?pageIndex=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllContracts_WithInvalidPagination_ReturnsBadRequest()
        {
            // pageIndex <= 0 -> BadRequest theo PaginationHelper
            var response = await _client.GetAsync("/contracts?pageIndex=0&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAllContracts_WithValidRequest_ReturnsOkAndData()
        {
            SeedContracts();

            var response = await _client.GetAsync("/contracts?pageIndex=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetAllContractsResponse>();
            result.Should().NotBeNull();
            result!.Contracts.Should().NotBeNull();

            var page = result.Contracts;
            page.PageIndex.Should().Be(1);
            page.PageSize.Should().Be(10);
            page.TotalItems.Should().BeGreaterThan(0);
            page.Data.Should().NotBeNull().And.NotBeEmpty();

            // Optional: assert one item content
            var any = page.Data.First();
            any.Id.Should().BeGreaterThan(0);
            any.ContractCode.Should().NotBeNullOrEmpty();
        }

        private class GetAllContractsResponse
        {
            public PaginatedResult<ContractResponse> Contracts { get; set; } = default!;
        }
    }
}
