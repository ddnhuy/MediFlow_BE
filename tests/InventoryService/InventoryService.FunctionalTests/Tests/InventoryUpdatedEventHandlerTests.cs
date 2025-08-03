using BuildingBlocks.Strings;
using Inventory.Application.Data;
using Inventory.Application.DomainEventsHandler.InventoryUpdated;
using Inventory.Domain.DomainEvents;
using Inventory.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class InventoryUpdatedEventHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockDbContext;
        private readonly Mock<ILogger<InventoryUpdatedEventHandler>> _mockLogger;
        private readonly InventoryUpdatedEventHandler _handler;

        public InventoryUpdatedEventHandlerTests()
        {
            _mockDbContext = new Mock<IApplicationDbContext>();
            _mockLogger = new Mock<ILogger<InventoryUpdatedEventHandler>>();
            _handler = new InventoryUpdatedEventHandler(_mockDbContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenInventoryDetailDoesNotExist_ShouldCreateNew()
        {
            // Arrange
            var notification = new InventoryUpdatedEvent(
                MedicineId: 1,
                MedicineBatchId: 1,
                BatchNumber: "BATCH001",
                WarehouseId: 1,
                Quantity: 10,
                CostPrice: 5.0m,
                UnitPrice: 7.5m
            );

            var inventoryDetails = new List<InventoryDetail>();
            var inventoryHistories = new List<InventoryHistory>();

            _mockDbContext.Setup(x => x.InventoryDetails)
                .ReturnsDbSet(inventoryDetails);

            _mockDbContext.Setup(x => x.InventoryHistories)
                .ReturnsDbSet(inventoryHistories);

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _mockDbContext.Verify(x => x.InventoryDetails.AddAsync(
                It.Is<InventoryDetail>(d =>
                    d.MedicineBatchId == notification.MedicineBatchId &&
                    d.WarehouseId == notification.WarehouseId &&
                    d.Quantity == notification.Quantity &&
                    d.CostPrice == notification.CostPrice
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _mockDbContext.Verify(x => x.InventoryHistories.AddAsync(
                It.Is<InventoryHistory>(h =>
                    h.MedicineId == notification.MedicineId &&
                    h.MedicineBatchId == notification.MedicineBatchId &&
                    h.WarehouseId == notification.WarehouseId &&
                    h.Quantity == notification.Quantity &&
                    h.UnitPrice == notification.UnitPrice &&
                    h.TransactionType == InventoryTransactionType.IMPORT &&
                    h.Description == $"Imported medicine from batch {notification.BatchNumber}"
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInventoryDetailExists_ShouldUpdateExisting()
        {
            // Arrange
            var notification = new InventoryUpdatedEvent(
                MedicineId: 1,
                MedicineBatchId: 1,
                BatchNumber: "BATCH001",
                WarehouseId: 1,
                Quantity: 10,
                CostPrice: 5.0m,
                UnitPrice: 7.5m
            );

            var existingDetail = new InventoryDetail
            {
                MedicineBatchId = 1,
                WarehouseId = 1,
                Quantity = 5,
                CostPrice = 5.0m
            };

            var medicineBatch = new MedicineBatch
            {
                Id = 1,
                MedicineId = 1,
                BatchNumber = "BATCH001",
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
                ImportPrice = 5.0m,
                CostPrice = 5.0m,
                SupplierId = 1
                // Add other required properties if needed
            };

            existingDetail.MedicineBatch = medicineBatch;

            var inventoryDetails = new List<InventoryDetail> { existingDetail };
            var inventoryHistories = new List<InventoryHistory>();
            var medicineBatches = new List<MedicineBatch> { medicineBatch };
            _mockDbContext.Setup(x => x.MedicineBatches)
                .ReturnsDbSet(medicineBatches);

            _mockDbContext.Setup(x => x.InventoryDetails)
                .ReturnsDbSet(inventoryDetails);

            _mockDbContext.Setup(x => x.InventoryHistories)
                .ReturnsDbSet(inventoryHistories);

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            Assert.Equal(15, existingDetail.Quantity); // 5 + 10

            _mockDbContext.Verify(x => x.InventoryDetails.Update(
                It.Is<InventoryDetail>(d =>
                    d.MedicineBatchId == notification.MedicineBatchId &&
                    d.WarehouseId == notification.WarehouseId &&
                    d.Quantity == 15 // Original 5 + new 10
                )
            ), Times.Once);

            _mockDbContext.Verify(x => x.InventoryHistories.AddAsync(
                It.Is<InventoryHistory>(h =>
                    h.MedicineId == notification.MedicineId &&
                    h.MedicineBatchId == notification.MedicineBatchId &&
                    h.WarehouseId == notification.WarehouseId &&
                    h.Quantity == notification.Quantity &&
                    h.UnitPrice == notification.UnitPrice &&
                    h.TransactionType == InventoryTransactionType.IMPORT
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogInformation()
        {
            // Arrange
            var notification = new InventoryUpdatedEvent(
                MedicineId: 1,
                MedicineBatchId: 1,
                BatchNumber: "BATCH001",
                WarehouseId: 1,
                Quantity: 10,
                CostPrice: 5.0m,
                UnitPrice: 7.5m
            );

            var inventoryDetails = new List<InventoryDetail>();
            var inventoryHistories = new List<InventoryHistory>();

            _mockDbContext.Setup(x => x.InventoryDetails)
                .ReturnsDbSet(inventoryDetails);

            _mockDbContext.Setup(x => x.InventoryHistories)
                .ReturnsDbSet(inventoryHistories);

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            //_mockLogger.Verify(
            //    x => x.Log(
            //        LogLevel.Information,
            //        It.IsAny<EventId>(),
            //        It.Is<It.IsAnyType>((v, t) => true),
            //        It.IsAny<Exception>(),
            //        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            //    Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
                Times.Once);
        }
    }
}