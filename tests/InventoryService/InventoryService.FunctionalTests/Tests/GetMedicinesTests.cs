using BuildingBlocks.Pagination;
using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetMedicinesTests : BaseFunctionalTest
    {
        public GetMedicinesTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicines_WithValidPagination_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetMedicines_WithSearchKeyword_ReturnsFilteredResults()
        {
            // Arrange
            var searchKeyword = "COVID"; // Should match "COVID-19 Vaccine"
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}&searchKeyword={searchKeyword}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();

            // Verify that all returned medicines contain the search keyword
            foreach (var medicine in result.Medicines.Data)
            {
                medicine.MedicineName.Should().Contain(searchKeyword);
            }
        }

        [Fact]
        public async Task GetMedicines_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };
            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicines_WithMedicineCodeSearch_ReturnsFilteredResults()
        {
            // Arrange
            var searchKeyword = "VAC-001"; // Should match specific medicine code
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}&searchKeyword={searchKeyword}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();

            // Verify that returned medicine has the correct code
            var medicine = result.Medicines.Data.First();
            medicine.MedicineCode.Should().Be(searchKeyword);
        }

        [Fact]
        public async Task GetMedicines_WithEmptySearchKeyword_ReturnsAllResults()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}&searchKeyword=");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetMedicines_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = -1, PageSize = 0 }; // Invalid pagination
            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMedicines_WithUnitPrice_ReturnsCorrectPrice()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();

            // Find a medicine that should have a price (COVID-19 Vaccine with ID 1)
            var covidVaccine = result.Medicines.Data.FirstOrDefault(m => m.MedicineCode == "VAC-001");
            if (covidVaccine != null)
            {
                covidVaccine.UnitPrice.Should().Be(625000m); // Expected price from seed data
            }
        }

        [Fact]
        public async Task GetMedicines_WithMedicineWithoutPrice_ReturnsNullPrice()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();

            // Find a medicine that should not have a price (Isomina Vaccine)
            var isominaVaccine = result.Medicines.Data.FirstOrDefault(m => m.MedicineCode == "ISO123");
            if (isominaVaccine != null)
            {
                isominaVaccine.UnitPrice.Should().BeNull(); // Should be null as no price exists
            }
        }
    }
}