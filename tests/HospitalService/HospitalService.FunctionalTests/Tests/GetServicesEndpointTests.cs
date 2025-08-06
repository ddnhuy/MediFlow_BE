using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetServicesEndpointTests : BaseFunctionalTest
    {
        public GetServicesEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetServices_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/services");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        public async Task GetServices_InvalidPagination_Returns400(int pageIndex, int pageSize)
        {
            // Arrange
            var url = $"/services?PageIndex={pageIndex}&PageSize={pageSize}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetServices_WithSearchTerm_Returns200()
        {
            // Arrange
            var searchTerm = "test";
            var url = $"/services?PageIndex=1&PageSize=10&searchTerm={searchTerm}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            Assert.NotNull(node);
            Assert.NotNull(node["services"]);
            Assert.NotNull(node["services"]["data"]);
        }

        [Fact]
        public async Task GetServices_WithEmptySearchTerm_Returns200()
        {
            // Arrange
            var url = "/services?PageIndex=1&PageSize=5&searchTerm=";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 20)]
        public async Task GetServices_DifferentPageSizes_Returns200(int pageIndex, int pageSize)
        {
            // Arrange
            var url = $"/services?PageIndex={pageIndex}&PageSize={pageSize}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            Assert.Equal(pageIndex, node["services"]["pageIndex"]?.GetValue<int>());
            Assert.Equal(pageSize, node["services"]["pageSize"]?.GetValue<int>());
        }
    }
}
