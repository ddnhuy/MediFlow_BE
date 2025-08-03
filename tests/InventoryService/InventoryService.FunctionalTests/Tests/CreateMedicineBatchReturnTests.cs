using BuildingBlocks.Strings.Enums;
using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Medicines.Commands.ReturnMedicineBatch;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Data;
using InventoryService.FunctionalTests.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryService.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class CreateMedicineBatchReturnTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        public CreateMedicineBatchReturnTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            SeedTestData();
        }       

        private void SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clear existing data
            dbContext.MedicineBatchReturnDetails.RemoveRange(dbContext.MedicineBatchReturnDetails);
            dbContext.MedicineBatchReturns.RemoveRange(dbContext.MedicineBatchReturns);
            dbContext.MedicineBatches.RemoveRange(dbContext.MedicineBatches);
            dbContext.SaveChanges();

            // Seed Medicine Batch first (required for foreign key)
            var medicineBatch = new MedicineBatch
            {
                Id = 10,
                MedicineId = 1, // Assuming medicine with ID 1 exists
                BatchNumber = "BATCH001",
                SupplierId = 1,
                ManufacturerId = 1,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)),
                Status = MedicineBatchStatus.IsActive,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };    
            
            var medicineBatch2 = new MedicineBatch
            {
                Id = 11,
                MedicineId = 1, // Assuming medicine with ID 1 exists
                BatchNumber = "BATCH011",
                ManufacturerId = 1,
                SupplierId = 2,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(100)),
                Status = MedicineBatchStatus.IsActive,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };

            dbContext.MedicineBatches.Add(medicineBatch);
            dbContext.MedicineBatches.Add(medicineBatch2);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RT001",
                Reason: "Expired medicine",
                ReceiverName: "John Doe",
                ReceiverEmail: "byte050403@gmail.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 10,
                        BatchNumber: "EXPIRED001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                        Quantity: 50
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateMedicineBatchReturnResponse>();
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RT001",
                Reason: "Expired medicine",
                ReceiverName: "John Doe",
                ReceiverEmail: "john.doe@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 11,
                        BatchNumber: "EXPIRED001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                        Quantity: 50
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithNonExpiredBatch_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RT009",
                Reason: "Non-expired medicine",
                ReceiverName: "John Doe",
                ReceiverEmail: "john.doe@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 11, 
                        BatchNumber: "EXPIRED001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                        Quantity: 50
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicineBatchReturn_WithDifferentSuppliers_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateMedicineBatchReturnCommand(
                ReturnCode: "RT002",
                Reason: "Expired medicine from different suppliers",
                ReceiverName: "John Doe",
                ReceiverEmail: "john.doe@example.com",
                ReceiverPhone: "0123456789",
                Details: new List<MedicineBatchReturnDetailDto>
                {
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 10, // SupplierId = 1
                        BatchNumber: "BATCH001",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)),
                        Quantity: 50
                    ),
                    new MedicineBatchReturnDetailDto(
                        MedicineBatchId: 11, // SupplierId = 2 (different supplier)
                        BatchNumber: "BATCH012",
                        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)),
                        Quantity: 30
                    )
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-batch-returns", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}