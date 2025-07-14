using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetListPatientsUnpaidServiceEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;

        public GetListPatientsUnpaidServiceEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        [Fact]
        public async Task GetListPatientsUnpaidService_Success()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            // Act
            var response = await _client.GetAsync("/patients/unpaid-services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetListPatientsUnpaidServiceResponse>();
            result.Should().NotBeNull();
            result!.Patients.Should().NotBeNull();
        }

        [Fact]
        public async Task GetListPatientsUnpaidService_Unauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/patients/unpaid-services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}