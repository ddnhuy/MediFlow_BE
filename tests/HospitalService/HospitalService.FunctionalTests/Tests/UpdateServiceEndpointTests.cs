using MediatR;
using NSubstitute.ReceivedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class UpdateServiceEndpointTests : BaseFunctionalTest
    {
        public UpdateServiceEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task UpdateService_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new
            {
                ServiceCode = "SVC001",
                ServiceName = "Updated Service",
                UnitPrice = 100.50m,
                DepartmentId = 1
            };

            var response = await _client.PutAsJsonAsync("/services/1", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateService_ValidRequest_Returns200()
        {
            var request = new
            {
                ServiceCode = "SVC001",
                ServiceName = "Updated Service",
                UnitPrice = 100.50m,
                DepartmentId = 1,
                Unit = "Test",
                StandardValue = "Test",
                Quantity = 1,
                EquipmentUsed = "test"
            };

            var response = await _client.PutAsJsonAsync("/services/1", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
            Assert.Equal(1, node["serviceId"]?.GetValue<int>());
        }

        [Fact]
        public async Task UpdateService_WithLongServiceName_Returns400()
        {
            var longServiceName = new string('a', 201);
            var request = new
            {
                ServiceCode = "SVC001",
                ServiceName = longServiceName,
                UnitPrice = 100.50m,
                DepartmentId = 1
            };

            var response = await _client.PutAsJsonAsync("/services/0", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateService_WithLongServiceId_Returns404()
        {
            var longServiceName = new string('a', 201);
            var request = new
            {
                ServiceCode = "SVC001",
                ServiceName = longServiceName,
                UnitPrice = 100.50m,
                DepartmentId = 1
            };

            var response = await _client.PutAsJsonAsync("/services/999", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
