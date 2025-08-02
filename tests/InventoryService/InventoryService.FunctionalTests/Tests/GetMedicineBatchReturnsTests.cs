using BuildingBlocks.Pagination;
using BuildingBlocks.Strings.Enums;
using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Data;
using Inventory.Application.Medicines.Queries.GetMedicineBatchReturns;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Data;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryService.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetMedicineBatchReturnTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public GetMedicineBatchReturnTests(FunctionalTestWebAppFactory factory) : base(factory) {
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
                Id = 1,
                MedicineId = 1, // Assuming medicine with ID 1 exists
                BatchNumber = "BATCH001",
                SupplierId = 1,
                ManufacturerId = 1,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                Status = MedicineBatchStatus.IsActive,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };

            // Seed Medicine Batch Return
            var testReturn = new MedicineBatchReturn
            {
                Id = 1,
                ReturnCode = "RT001",
                Reason = "Expired medicine",
                ReceiverName = "John Doe",
                ReceiverEmail = "john.doe@example.com",
                ReceiverPhone = "0123456789",
                Status = MedicineBatchReturnStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            // Seed Medicine Batch Return Detail
            var testDetail = new MedicineBatchReturnDetail
            {
                Id = 1,
                MedicineBatchReturnId = 1,
                MedicineBatchId = 1,
                BatchNumber = "BATCH001",
                ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                Quantity = 50
            };

            dbContext.MedicineBatches.Add(medicineBatch);
            dbContext.MedicineBatchReturns.Add(testReturn);
            dbContext.MedicineBatchReturnDetails.Add(testDetail);
            dbContext.SaveChanges();
        }

        // Get All Medicine Batch Returns Tests
        [Fact]
        public async Task GetMedicineBatchReturns_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchReturnsResult>();
            result.Should().NotBeNull();
            result!.MedicineBatchReturns.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineBatchReturns_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineBatchReturns_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = -1, PageSize = 0 }; // Invalid pagination

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Get Medicine Batch Return By Id Tests
        [Fact]
        public async Task GetMedicineBatchReturnById_WithValidId_ReturnsOk()
        {
            // Arrange
            var returnId = 1; // Test data ID

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns/{returnId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineBatchReturnByIdResponse>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(returnId);         
            result.Details.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetMedicineBatchReturnById_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var returnId = 1;

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns/{returnId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineBatchReturnById_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var returnId = 0; // Invalid ID (0 or negative)

            // Act
            var response = await _client.GetAsync($"/medicine-batch-returns/{returnId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}