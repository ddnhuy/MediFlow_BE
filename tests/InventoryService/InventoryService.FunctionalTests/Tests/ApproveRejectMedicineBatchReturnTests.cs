using BuildingBlocks.Strings.Enums;
using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Medicines.Commands.ApproveMedicineBatchReturn;
using Inventory.Application.Medicines.Commands.RejectMedicineBatchReturn;
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
    public class ApproveRejectMedicineBatchReturnTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        public ApproveRejectMedicineBatchReturnTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                Status = MedicineBatchStatus.IsActive,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };

            // Seed Medicine Batch Return with pending status
            var medicineBatchReturn = new MedicineBatchReturn
            {
                Id = 1,
                ReturnCode = "RT001",
                Reason = "Expired medicine",
                ReceiverName = "John Doe",
                ReceiverEmail = "john.doe@example.com",
                ReceiverPhone = "0123456789",
                Status = MedicineBatchReturnStatus.Pending,
                ApprovalToken = "test-approval-token-123",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            // Seed Medicine Batch Return Detail
            var medicineBatchReturnDetail = new MedicineBatchReturnDetail
            {
                Id = 1,
                MedicineBatchReturnId = 1,
                MedicineBatchId = 10,
                BatchNumber = "BATCH001",
                ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                Quantity = 50
            };

            dbContext.MedicineBatches.Add(medicineBatch);
            dbContext.MedicineBatchReturns.Add(medicineBatchReturn);
            dbContext.MedicineBatchReturnDetails.Add(medicineBatchReturnDetail);
            dbContext.SaveChanges();
        }

        // Approve Tests
        [Fact]
        public async Task ApproveMedicineBatchReturn_WithValidToken_ReturnsOk()
        {
            // Arrange
            var returnId = 1;
            var request = new ApproveMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ApproveMedicineBatchReturn_WhenUnauthorized_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var returnId = 1;
            var request = new ApproveMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ApproveMedicineBatchReturn_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var returnId = 1;
            var request = new ApproveMedicineBatchReturnRequest("invalid-token");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ApproveMedicineBatchReturn_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var returnId = 999; // Non-existent ID
            var request = new ApproveMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ApproveMedicineBatchReturn_WithAlreadyProcessed_ReturnsBadRequest()
        {
            // Arrange - First approve the return
            var returnId = 1;
            var request = new ApproveMedicineBatchReturnRequest("test-approval-token-123");
            await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Act - Try to approve again
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Reject Tests
        [Fact]
        public async Task RejectMedicineBatchReturn_WithValidToken_ReturnsOk()
        {
            // Arrange
            var returnId = 1;
            var request = new RejectMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RejectMedicineBatchReturn_WhenUnauthorized_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var returnId = 1;
            var request = new RejectMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RejectMedicineBatchReturn_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var returnId = 1;
            var request = new RejectMedicineBatchReturnRequest("invalid-token");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RejectMedicineBatchReturn_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var returnId = 999; // Non-existent ID
            var request = new RejectMedicineBatchReturnRequest("test-approval-token-123");

            // Act
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RejectMedicineBatchReturn_WithAlreadyProcessed_ReturnsBadRequest()
        {
            // Arrange - First reject the return
            var returnId = 1;
            var request = new RejectMedicineBatchReturnRequest("test-approval-token-123");
            await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Act - Try to reject again
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Cross-validation tests
        [Fact]
        public async Task ApproveMedicineBatchReturn_AfterReject_ReturnsBadRequest()
        {
            // Arrange - First reject the return
            var returnId = 1;
            var rejectRequest = new RejectMedicineBatchReturnRequest("test-approval-token-123");
            await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", rejectRequest);

            // Act - Try to approve after reject
            var approveRequest = new ApproveMedicineBatchReturnRequest("test-approval-token-123");
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", approveRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RejectMedicineBatchReturn_AfterApprove_ReturnsBadRequest()
        {
            // Arrange - First approve the return
            var returnId = 1;
            var approveRequest = new ApproveMedicineBatchReturnRequest("test-approval-token-123");
            await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/approve", approveRequest);

            // Act - Try to reject after approve
            var rejectRequest = new RejectMedicineBatchReturnRequest("test-approval-token-123");
            var response = await _client.PostAsJsonAsync($"/medicine-batch-returns/{returnId}/reject", rejectRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}