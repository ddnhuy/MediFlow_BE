using HospitalService.Application.Services.HospitalServices.Commands;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using HospitalService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json.Nodes;

namespace HospitalService.FunctionalTests.Tests
{
    public class AddServicesToGroupEndpointTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        public AddServicesToGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AddServicesToGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { ServiceIds = new[] { 1, 2, 3 } };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToGroup_InvalidRequest_Returns400()
        {
            // Arrange
            var request = new { ServiceIds = new int[] { } }; // Empty array should be invalid

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToGroup_ValidRequest_Returns200()
        {
            // Arrange
            var request = new { ServiceIds = new[] { 1, 2} };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }

        [Fact]
        public async Task AddServicesToGroup_WhenExceptionOccurs_ShouldRollbackTransaction()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create a service group first
            var serviceGroup = new ServiceGroup
            {
                GroupName = "Test Group",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };
            await dbContext.ServiceGroups.AddAsync(serviceGroup);
            await dbContext.SaveChangesAsync();

            // Mock the service group repository to throw an exception
            var mockServiceGroupRepository = new Mock<IServiceGroupRepository>();
            mockServiceGroupRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Simulated error"));

            var mockServiceGroupServiceRepository = new Mock<IServiceGroupServiceRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AddServicesToGroupCommand>>();

            var handler = new AddServicesToGroupCommandHandler(
                mockServiceGroupRepository.Object,
                mockServiceGroupServiceRepository.Object,
                mockUnitOfWork.Object,
                mockLogger.Object
            );

            var command = new AddServicesToGroupCommand(
                ServiceGroupId: serviceGroup.Id,
                ServiceIds: new List<int> { 1, 2, 3 }
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

            // Verify that rollback was called
            mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify that commit was never called
            mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddServicesToGroup_InvalidId_Returns404()
        {
            // Arrange
            var request = new { ServiceIds = new[] { 1, 2 } };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/9999/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
