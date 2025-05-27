using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Helpers;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class ApplicationDbContextTests
    {
        private readonly Mock<ICurrentUserHelper> _userHelperMock;
        private readonly Mock<ILogger<ApplicationDbContext>> _loggerMock;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Setup mocks
            _userHelperMock = new Mock<ICurrentUserHelper>();
            _loggerMock = new Mock<ILogger<ApplicationDbContext>>();

            // Create context with mocked dependencies
            _context = new ApplicationDbContext(
                options,
                _userHelperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAddingReception_ShouldSetCreatedAndUpdatedFields()
        {
            // Arrange
            const int userId = 123;
            _userHelperMock.Setup(x => x.UserId).Returns(userId);

            var reception = new Reception
            {
                // Set required properties for Reception
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            // Act
            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(userId, reception.CreatedBy);
            Assert.Equal(userId, reception.LastUpdatedBy);
            Assert.True(reception.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
            Assert.True(reception.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAddingScreeningEvaluationReport_ShouldSetCreatedAndUpdatedFields()
        {
            // Arrange
            const int userId = 123;
            _userHelperMock.Setup(x => x.UserId).Returns(userId);

            var report = new ScreeningEvaluationReport
            {
                // Set required properties for ScreeningEvaluationReport
                ReceptionId = 1,
            };

            // Act
            _context.ScreeningEvaluationReports.Add(report);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(userId, report.CreatedBy);
            Assert.Equal(userId, report.LastUpdatedBy);
            Assert.True(report.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
            Assert.True(report.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task SaveChangesAsync_WhenUpdatingEntity_ShouldUpdateLastUpdatedFields()
        {
            // Arrange
            const int userId = 123;
            _userHelperMock.Setup(x => x.UserId).Returns(userId);

            var reception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();

            // Act
            reception.ReceptionDate = DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(userId, reception.CreatedBy);
            Assert.Equal(userId, reception.LastUpdatedBy);
            Assert.True(reception.CreatedAt < reception.LastUpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenUserIdIsZero_ShouldUseDefaultUserId()
        {
            // Arrange
            _userHelperMock.Setup(x => x.UserId).Returns(0);

            var reception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            // Act
            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, reception.CreatedBy);
            Assert.Equal(1, reception.LastUpdatedBy);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenExceptionOccurs_ShouldLogErrorAndRethrow()
        {
            // Arrange
            _userHelperMock.Setup(x => x.UserId).Returns(123);

            var reception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();

            // Simulate database error by disposing context
            _context.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(
                () => _context.SaveChangesAsync()
            );

            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldLogDebugInformation()
        {
            // Arrange
            const int userId = 123;
            _userHelperMock.Setup(x => x.UserId).Returns(userId);

            var reception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            // Act
            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) =>
                        o.ToString().Contains("Current user ID") ||
                        o.ToString().Contains("Updated entity")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.AtLeast(2)
            );
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAddingMultipleEntities_ShouldSetFieldsForAll()
        {
            // Arrange
            const int userId = 123;
            _userHelperMock.Setup(x => x.UserId).Returns(userId);

            var reception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow
            };

            var report = new ScreeningEvaluationReport
            {
                ReceptionId = 1
            };

            // Act
            _context.Receptions.Add(reception);
            _context.ScreeningEvaluationReports.Add(report);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(userId, reception.CreatedBy);
            Assert.Equal(userId, reception.LastUpdatedBy);
            Assert.Equal(userId, report.CreatedBy);
            Assert.Equal(userId, report.LastUpdatedBy);
        }
    }
}