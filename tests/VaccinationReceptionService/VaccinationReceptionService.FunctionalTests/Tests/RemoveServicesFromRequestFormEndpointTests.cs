using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class RemoveServicesFromRequestFormEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;

        public RemoveServicesFromRequestFormEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = 1,
                    ReceptionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

                // Create RequestFormService if not exists
                var requestFormService = dbContext.ServiceRequestDetails
                    .FirstOrDefault(rfs => rfs.ReceptionId == reception.Id && rfs.ServiceId == TestServiceId);
                if (requestFormService == null)
                {
                    requestFormService = new ServiceRequestDetail
                    {
                        ReceptionId = TestReceptionId,
                        RequestNumber = "REQ001",
                        ServiceId = TestServiceId,
                        Quantity = 1,
                        UnitPrice = 100,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    };
                    dbContext.ServiceRequestDetails.Add(requestFormService);
                }
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithValidData_ReturnsOk()
        {
            // Arrange
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<RemoveServicesFromRequestFormResponse>();
            result.Should().NotBeNull();
            result!.RequestFormId.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task RemoveServicesFromRequestForm_WithEmptyServiceIds_ReturnsBadRequest()
        {
            // Arrange
            var serviceIds = new List<int>(); // Empty list

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithInvalidReceptionId_ReturnsNotFound()
        {
            // Arrange
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"/request-forms/9999/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithInvalidServiceId_ReturnsNotFound()
        {
            // Arrange
            var serviceIds = new List<int> { 99999 };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}