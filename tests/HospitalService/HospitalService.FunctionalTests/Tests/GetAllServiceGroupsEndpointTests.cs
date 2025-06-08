using BuildingBlocks.Pagination;
using HospitalService.Application.DTOs;
using HospitalService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetAllServiceGroupsEndpointTests : BaseFunctionalTest
    {
        public GetAllServiceGroupsEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetAllServiceGroups_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/servicegroups/all");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllServiceGroups_WithGroups_Returns200()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/servicegroups/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var serviceGroups = node?["serviceGroups"]?.AsArray();

            Assert.NotNull(serviceGroups);
            Assert.True(serviceGroups.Count > 0);
        }

        [Theory]
        [InlineData("cơ bản")]
        public async Task GetAllServiceGroups_WithSearchTerm_ReturnsFilteredResults(string searchTerm)
        {
            // Arrange
            var url = $"/servicegroups/all?searchTerm={searchTerm}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var serviceGroups = node?["serviceGroups"]?.AsArray();

            Assert.NotNull(serviceGroups);
        }

        [Fact]
        public async Task GetAllServiceGroups_WithEmptySearchTerm_ReturnsAllResults()
        {
            // Arrange
            var url = "/servicegroups/all?searchTerm=";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var serviceGroups = node?["serviceGroups"]?.AsArray();

            Assert.NotNull(serviceGroups);
            Assert.True(serviceGroups.Count > 0);
        }

        [Fact]
        public async Task GetAllServiceGroups_ResponseHasCorrectStructure()
        {
            // Arrange
            var url = "/servicegroups/all";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            Assert.NotNull(node);
            Assert.NotNull(node["serviceGroups"]);
            var serviceGroups = node["serviceGroups"].AsArray();

        }
    }
}