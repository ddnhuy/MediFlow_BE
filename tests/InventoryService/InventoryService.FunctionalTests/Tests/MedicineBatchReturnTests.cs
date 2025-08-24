using FluentAssertions;
using Inventory.Application.Medicines.Commands.ReturnMedicineBatch;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using BuildingBlocks.Strings.Enums;

namespace Inventory.FunctionalTests.Tests
{
    public class MedicineBatchReturnTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public MedicineBatchReturnTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithValidData_ReturnsCreated()
        {
            // Arrange - First create an expired medicine batch
            using var scope = _factory.Services.CreateScope();
            var dbContext =
                scope.ServiceProvider.GetRequiredService<Inventory.Infrastructure.Data.ApplicationDbContext>();

            // Create medicine batch with expired date
            var batch = new Inventory.Domain.Models.MedicineBatch
            {
                MedicineId = 1, // Use existing medicine from seeder
                BatchNumber = "BATCH001",
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), // Expired 5 days ago
                ImportPrice = 100.00m,
                CostPrice = 95.00m,
                SupplierId = 1, // Use existing supplier
                Status = MedicineBatchStatus.IsActive,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };

            dbContext.MedicineBatches.Add(batch);
            await dbContext.SaveChangesAsync();

            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET001",
                Reason: "Expired medicine batch",
                ReceiverName: "John Doe",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: batch.Id,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                        Quantity: 10.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithEmptyReturnCode_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "",
                Reason: "Expired medicine batch",
                ReceiverName: "John Doe",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 1,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        Quantity: 10.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithEmptyReceiverName_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET002",
                Reason: "Expired medicine batch",
                ReceiverName: "",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 1,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        Quantity: 10.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET003",
                Reason: "Expired medicine batch",
                ReceiverName: "John Doe",
                ReceiverEmail: "invalid-email",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 1,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        Quantity: 10.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithEmptyDetails_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET004",
                Reason: "Expired medicine batch",
                ReceiverName: "John Doe",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>()
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithNegativeQuantity_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET005",
                Reason: "Expired medicine batch",
                ReceiverName: "John Doe",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 1,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        Quantity: -5.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RET006",
                Reason: "Unauthorized return",
                ReceiverName: "John Doe",
                ReceiverEmail: "receiver@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 1,
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        Quantity: 10.0m
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
