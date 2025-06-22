using HospitalFee.FunctionalTests.Abstractions;
using HospitalFee.FunctionalTests.DataTest;
using HospitalFee.FunctionalTests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.HospitalFeeEndpoints;
using VaccinationReception.Domain.DTOs;
using VaccinationReception.Domain.Models;

namespace HospitalFee.FunctionalTests.Tests
{
    public class GetPaymentDetailsEndpointTests : BaseFunctionalTest
    {
        private string _testToken;

        public GetPaymentDetailsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory) 
        {
            _testToken = TokenHelper.GenerateTestToken();
        }
        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        //[Fact]
        //public async Task GetPaymentDetails_WhenPaymentExists_ReturnsOkWithDetails()
        //{
        //    SetAuthHeader();
        //    // Arrange
        //    var payment = await TestDataFactory.SeedPaidPaymentScenarioAsync(_dbContext);
        //    var serviceId = payment.PaymentDetails.First().ServiceRequestDetail!.ServiceId;

        //    MockGetServicesByIds(
        //        new List<int> { 101 },
        //        new List<ServiceResponse>
        //        {
        //            new ServiceResponse(101, "SVC101", "Test Service", 50000m, 1)
        //        }
        //    );

        //    // Act
        //    var response = await _client.GetAsync("/payments/1/details");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadFromJsonAsync<GetPaymentDetailsResponse>();

        //    Assert.NotNull(content);
        //    Assert.Equal(1, content.Payment.Id);
        //    Assert.Single(content.PaymentDetails);
        //    Assert.Equal("SVC101", content.PaymentDetails.First().ServiceCode);
        //}

        [Fact]
        public async Task GetPaymentDetails_WithoutToken_ReturnsUnauthorized()
        {
            
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/payments/1/details");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPaymentDetails_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var invalidPaymentId = 0;

            // Act
            var response = await _client.GetAsync($"/payments/{invalidPaymentId}/details");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
