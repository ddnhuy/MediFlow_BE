using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using VaccinationReception.Application.Vaccinations.Queries.GetNearestExpiryMedicineBatchWithMedicineId;
using NSubstitute;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetNearestExpiryMedicineBatchWithMedicineIdEndpointTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        private readonly string _testToken;
        private const int TestMedicineId = 1;

        public GetNearestExpiryMedicineBatchWithMedicineIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetNearestExpiryMedicineBatch_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var expectedBatch = new GetNearestExpiryMedicineBatchItem
            {
                MedicineBatchId = 123,
                MedicineBatchNumber = "BATCH-001",
                MedicineId = TestMedicineId,
                MedicineName = "Test Medicine",
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
            };

            var expectedResponse = new GetNearestExpiryMedicineBatchResponse
            {
                MedicineBatches = new List<GetNearestExpiryMedicineBatchItem> { expectedBatch },
                RequestId = Guid.NewGuid().ToString(),
                RequestedAt = DateTime.UtcNow,
                IsSuccess = true,
                ErrorMessage = null
            };

            _factory.InventoryServiceMock!
                .GetNearestExpiryMedicineBatchAsync(TestMedicineId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            // Act
            var response = await _client.GetAsync($"/vaccination/nearest-expiry-medicine-batch/{TestMedicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetNearestExpiryMedicineBatchWithMedicineIdResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
            result.MedicineBatches.Should().NotBeNull();
            result.MedicineBatches.Should().HaveCount(1);
            var batch = result.MedicineBatches.First();
            batch.MedicineBatchId.Should().Be(expectedBatch.MedicineBatchId);
            batch.MedicineBatchNumber.Should().Be(expectedBatch.MedicineBatchNumber);
            batch.MedicineId.Should().Be(expectedBatch.MedicineId);
            batch.MedicineName.Should().Be(expectedBatch.MedicineName);
            batch.ExpiryDate.Should().Be(expectedBatch.ExpiryDate);
        }

        [Fact]
        public async Task GetNearestExpiryMedicineBatch_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/nearest-expiry-medicine-batch/{TestMedicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetNearestExpiryMedicineBatch_WithNotFound_ReturnsError()
        {
            // Arrange
            var errorResponse = new GetNearestExpiryMedicineBatchResponse
            {
                MedicineBatches = new List<GetNearestExpiryMedicineBatchItem>(), // Empty list for not found
                RequestId = Guid.NewGuid().ToString(),
                RequestedAt = DateTime.UtcNow,
                IsSuccess = false,
                ErrorMessage = "Not found"
            };

            _factory.InventoryServiceMock!
                .GetNearestExpiryMedicineBatchAsync(TestMedicineId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(errorResponse));

            // Act
            var response = await _client.GetAsync($"/vaccination/nearest-expiry-medicine-batch/{TestMedicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetNearestExpiryMedicineBatchWithMedicineIdResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Not found");
            result.MedicineBatches.Should().BeEmpty();
        }
    }
}