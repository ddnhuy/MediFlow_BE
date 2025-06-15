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