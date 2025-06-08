using BuildingBlocks.Pagination;
using HospitalService.Application.DTOs;
using HospitalService.FunctionalTests.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetServiceGroupsEndpointTests : BaseFunctionalTest
    {
        public GetServiceGroupsEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetServiceGroups_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/servicegroups");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        public async Task GetServiceGroups_InvalidPagination_Returns400(int pageIndex, int pageSize)
        {
            // Arrange
            var url = $"/servicegroups?PageIndex={pageIndex}&PageSize={pageSize}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceGroups_ValidRequest_Returns200()
        {
            // Arrange
            var url = "/servicegroups?PageIndex=1&PageSize=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            var data = node?["serviceGroups"]?["data"]?.AsArray();

            Assert.NotNull(data);
            Assert.True(data.Count > 0);
        }

        [Fact]
        public async Task GetServiceGroups_WithSearchTerm_ReturnsFilteredResults()
        {
            // Arrange
            var searchTerm = "bản";
            var url = $"/servicegroups?PageIndex=1&PageSize=10&searchTerm={searchTerm}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var data = node?["serviceGroups"]?["data"]?.AsArray();

            Assert.NotNull(data);
            Assert.True(data.Count > 0);
        }
    }
}