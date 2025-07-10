using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetLatestReceptionIdByPatientIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 123;
        private const int TestReceptionId = 555;

        public GetLatestReceptionIdByPatientIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            var now = DateTime.UtcNow;

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var oldReceptions = dbContext.Receptions.Where(r => r.PatientId == TestPatientId).ToList();
            dbContext.Receptions.RemoveRange(oldReceptions);

            dbContext.Receptions.Add(new Reception
            {
                Id = TestReceptionId,
                PatientId = TestPatientId,
                ServiceTypeId = 1,
                ReceptionDate = now,
                CreatedAt = now,
                CreatedBy = 1,
                LastUpdatedAt = now,
                LastUpdatedBy = 1
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetLatestReceptionIdByPatientId_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/patients/{TestPatientId}/latest-reception-id");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetLatestReceptionIdByPatientId_WithValidData_ReturnsOk()
        {
            var response = await _client.GetAsync($"/patients/{TestPatientId}/latest-reception-id");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LatestReceptionIdResponse>();
            result.Should().NotBeNull();
            result!.ReceptionId.Should().Be(TestReceptionId);
        }

        [Fact]
        public async Task GetLatestReceptionIdByPatientId_WithInvalidPatientId_ReturnsBadRequest()
        {
            var response = await _client.GetAsync($"/patients/0/latest-reception-id");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetLatestReceptionIdByPatientId_WithNotFoundData_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"/patients/99999/latest-reception-id");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public class LatestReceptionIdResponse
        {
            public int ReceptionId { get; set; }
        }
    }
}
