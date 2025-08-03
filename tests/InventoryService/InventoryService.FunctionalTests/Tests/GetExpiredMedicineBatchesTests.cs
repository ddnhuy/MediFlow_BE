using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetExpiredMedicineBatchesTests : BaseFunctionalTest
    {
        public GetExpiredMedicineBatchesTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetExpiredMedicineBatches_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/medicines/expired-batches?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetExpiredMedicineBatchesResponse>();
            result.Should().NotBeNull();
            result!.ExpiredBatches.Should().NotBeNull();

            // Validate pagination structure
            result.ExpiredBatches.PageIndex.Should().Be(1);
            result.ExpiredBatches.PageSize.Should().Be(10);
            result.ExpiredBatches.TotalItems.Should().BeGreaterThanOrEqualTo(0);
            result.ExpiredBatches.TotalPages.Should().BeGreaterThanOrEqualTo(0);
            result.ExpiredBatches.HasPreviousPage.Should().BeFalse(); // First page
            result.ExpiredBatches.HasNextPage.Should().Be(result.ExpiredBatches.TotalPages > 1);

            // Validate data structure if any expired batches exist
            if (result.ExpiredBatches.Data.Any())
            {
                var firstBatch = result.ExpiredBatches.Data.First();

                // Validate required properties
                firstBatch.MedicineId.Should().BeGreaterThan(0);
                firstBatch.MedicineBatchId.Should().BeGreaterThan(0);
                firstBatch.BatchNumber.Should().NotBeNullOrEmpty();
                firstBatch.MedicineName.Should().NotBeNullOrEmpty();
                firstBatch.Unit.Should().NotBeNullOrEmpty();
                firstBatch.SupplierId.Should().BeGreaterThan(0);

                // Validate business logic
                firstBatch.ExpiryDate.Should().BeBefore(DateOnly.FromDateTime(DateTime.UtcNow));
                firstBatch.CurrentQuantity.Should().BeGreaterThanOrEqualTo(0);

                // Validate optional properties (should not be null but can be empty)
                firstBatch.MedicineCode.Should().NotBeNull();
                firstBatch.SupplierName.Should().NotBeNull();
                firstBatch.ContactPerson.Should().NotBeNull();
                firstBatch.PhoneNumber.Should().NotBeNull();
                firstBatch.Email.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetExpiredMedicineBatches_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/medicines/expired-batches?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetExpiredMedicineBatches_WithInvalidPagination_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/medicines/expired-batches?pageIndex=-1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}