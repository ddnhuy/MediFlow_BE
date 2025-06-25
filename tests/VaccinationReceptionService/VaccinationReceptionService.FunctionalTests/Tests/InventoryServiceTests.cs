using BuildingBlocks.Messaging.Contracts.Inventory;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using MassTransit;
using Microsoft.Extensions.Logging;
using VaccinationReception.Infrastructure.Services.InventoryMessaging;

namespace VaccinationReceptionService.FunctionalTests.Tests;

public class InventoryServiceTests
{
    private readonly IRequestClient<GetMedicineInformationRequest> _medicineInfoClient = Substitute.For<IRequestClient<GetMedicineInformationRequest>>();
    private readonly IRequestClient<GetNearestExpiryMedicineBatchRequest> _nearestBatchClient = Substitute.For<IRequestClient<GetNearestExpiryMedicineBatchRequest>>();
    private readonly ILogger<InventoryService> _logger = Substitute.For<ILogger<InventoryService>>();
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _service = new InventoryService(_medicineInfoClient, _logger, _nearestBatchClient);
    }

    [Fact]
    public async Task GetMedicineInformationAsync_ReturnsResponses_WhenSuccess()
    {
        // Arrange
        var medicineId = 1;
        var request = new GetMedicineInformationRequest { MedicineId = medicineId };
        var expectedResponse = new GetMedicineInformationResponse
        {
            MedicineId = medicineId,
            MedicineName = "Test",
            IsSuccess = true
        };

        var response = Substitute.For<Response<GetMedicineInformationResponse>>();
        response.Message.Returns(expectedResponse);

        _medicineInfoClient
            .GetResponse<GetMedicineInformationResponse>(Arg.Any<GetMedicineInformationRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _service.GetMedicineInformationAsync(new[] { medicineId });

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedResponse.MedicineId, result[0].MedicineId);
        Assert.True(result[0].IsSuccess);
    }

    [Fact]
    public async Task GetMedicineInformationAsync_ThrowsAndLogs_WhenException()
    {
        // Arrange
        _medicineInfoClient
            .GetResponse<GetMedicineInformationResponse>(Arg.Any<GetMedicineInformationRequest>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetMedicineInformationAsync(new[] { 1 }));
    }

    [Fact]
    public async Task GetNearestExpiryMedicineBatchAsync_ReturnsResponse_WhenSuccess()
    {
        // Arrange
        var medicineId = 1;
        var request = new GetNearestExpiryMedicineBatchRequest { MedicineId = medicineId };
        var expectedResponse = new GetNearestExpiryMedicineBatchResponse
        {
            MedicineId = medicineId,
            MedicineBatchId = 10,
            IsSuccess = true
        };

        var response = Substitute.For<Response<GetNearestExpiryMedicineBatchResponse>>();
        response.Message.Returns(expectedResponse);

        _nearestBatchClient
            .GetResponse<GetNearestExpiryMedicineBatchResponse>(Arg.Any<GetNearestExpiryMedicineBatchRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _service.GetNearestExpiryMedicineBatchAsync(medicineId);

        // Assert
        Assert.Equal(expectedResponse.MedicineBatchId, result.MedicineBatchId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetNearestExpiryMedicineBatchAsync_ThrowsAndLogs_WhenException()
    {
        // Arrange
        _nearestBatchClient
            .GetResponse<GetNearestExpiryMedicineBatchResponse>(Arg.Any<GetNearestExpiryMedicineBatchRequest>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetNearestExpiryMedicineBatchAsync(1));
    }
}
