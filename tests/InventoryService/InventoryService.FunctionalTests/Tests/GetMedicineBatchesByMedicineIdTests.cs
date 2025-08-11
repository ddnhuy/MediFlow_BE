using FluentAssertions;
using Inventory.Application.Medicines.Queries.GetMedicineBatchesByMedicineId;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryService.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetMedicineBatchesByMedicineIdTests : BaseFunctionalTest
    {
        public GetMedicineBatchesByMedicineIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var medicineId = 1;
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
            result!.PaginatedResult.Should().NotBeNull();
            result.PaginatedResult.PageIndex.Should().Be(pageIndex);
            result.PaginatedResult.PageSize.Should().Be(pageSize);
            result.PaginatedResult.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithBatchNumberFilter_ReturnsFilteredResults()
        {
            // Arrange
            var medicineId = 1;
            var batchNumber = "BATCH001";
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}&batchNumber={batchNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
            result!.PaginatedResult.Should().NotBeNull();

            // If there are results, verify they contain the batch number filter
            if (result.PaginatedResult.Data.Any())
            {
                foreach (var batch in result.PaginatedResult.Data)
                {
                    batch.BatchNumber.Should().Contain(batchNumber, "all returned batches should contain the search term");
                }
            }
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithValidMedicineIdAndPagination_ReturnsCorrectPagination()
        {
            // Arrange
            var medicineId = 1;
            var pageIndex = 1;
            var pageSize = 5;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
            result!.PaginatedResult.PageIndex.Should().Be(pageIndex);
            result.PaginatedResult.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var medicineId = 1;
            var pageIndex = 1;
            var pageSize = 10;

            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var medicineId = 1;
            var invalidPageIndex = -1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={invalidPageIndex}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithInvalidPageSize_ReturnsBadRequest()
        {
            // Arrange
            var medicineId = 1;
            var pageIndex = 1;
            var invalidPageSize = 0;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={invalidPageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithNegativePageSize_ReturnsBadRequest()
        {
            // Arrange
            var medicineId = 1;
            var pageIndex = 1;
            var invalidPageSize = -5;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={invalidPageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithNonExistentMedicineId_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentMedicineId = 99999;
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{nonExistentMedicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithEmptyBatchNumberFilter_ReturnsAllBatches()
        {
            // Arrange
            var medicineId = 1;
            var emptyBatchNumber = "";
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}&batchNumber={emptyBatchNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
            result!.PaginatedResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineBatchesByMedicineId_WithCaseInsensitiveBatchNumberFilter_ReturnsMatchingResults()
        {
            // Arrange
            var medicineId = 1;
            var batchNumberLowerCase = "batch";
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var response = await _client.GetAsync($"/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches?pageIndex={pageIndex}&pageSize={pageSize}&batchNumber={batchNumberLowerCase}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchesByMedicineIdResult>();
            result.Should().NotBeNull();
        }
    }
}