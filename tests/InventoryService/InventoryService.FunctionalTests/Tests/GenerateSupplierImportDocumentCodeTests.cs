using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Data;
using Inventory.Application.Suppliers.Queries;
using Inventory.Domain.Models;
using InventoryService.FunctionalTests.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GenerateSupplierImportDocumentCodeQueryHandlerTests : BaseFunctionalTest
    {
        public GenerateSupplierImportDocumentCodeQueryHandlerTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }        

        [Fact]
        public async Task Handle_WithNoExistingDocuments_ShouldReturnFirstSequence()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";
            var yearPrefix = $"NK{today.Year}_";

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(x => x.SupplierImportDocuments)
                .ReturnsDbSet(new List<SupplierImportDocument>());

            var handler = new GenerateSupplierImportDocumentCodeQueryHandler(mockDbContext.Object);
            var query = new GenerateSupplierImportDocumentCodeQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{codePrefix}-001", result.DocumentCode);
            Assert.Equal($"{yearPrefix}001", result.DocumentNumber);
        }

        [Fact]
        public async Task Handle_WithExistingDocuments_ShouldReturnNextSequence()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";
            var yearPrefix = $"NK{today.Year}_";

            var existingDocuments = new List<SupplierImportDocument>
            {
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-001", DocumentNumber = $"{yearPrefix}001" },
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-002", DocumentNumber = $"{yearPrefix}002" },
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(x => x.SupplierImportDocuments)
                .ReturnsDbSet(existingDocuments);

            var handler = new GenerateSupplierImportDocumentCodeQueryHandler(mockDbContext.Object);
            var query = new GenerateSupplierImportDocumentCodeQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{codePrefix}-003", result.DocumentCode);
            Assert.Equal($"{yearPrefix}003", result.DocumentNumber);
        }

        [Fact]
        public async Task Handle_WithNonSequentialDocuments_ShouldReturnNextAfterHighest()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";
            var yearPrefix = $"NK{today.Year}_";

            var existingDocuments = new List<SupplierImportDocument>
            {
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-001", DocumentNumber = $"{yearPrefix}001" },
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-005", DocumentNumber = $"{yearPrefix}007" },
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-003", DocumentNumber = $"{yearPrefix}002" },
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(x => x.SupplierImportDocuments)
                .ReturnsDbSet(existingDocuments);

            var handler = new GenerateSupplierImportDocumentCodeQueryHandler(mockDbContext.Object);
            var query = new GenerateSupplierImportDocumentCodeQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{codePrefix}-006", result.DocumentCode);
            Assert.Equal($"{yearPrefix}008", result.DocumentNumber);
        }

        [Fact]
        public async Task Handle_WithDocumentsFromDifferentDates_ShouldOnlyCountToday()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";
            var yearPrefix = $"NK{today.Year}_";

            var yesterdayDate = today.AddDays(-1);
            string yesterdayString = $"{yesterdayDate.Year}{yesterdayDate.Month:D2}{yesterdayDate.Day:D2}";
            var yesterdayPrefix = $"PN{yesterdayString}";

            var existingDocuments = new List<SupplierImportDocument>
            {
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-001", DocumentNumber = $"{yearPrefix}001" },
                new SupplierImportDocument { DocumentCode = $"{yesterdayPrefix}-010", DocumentNumber = $"{yearPrefix}002" },
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(x => x.SupplierImportDocuments)
                .ReturnsDbSet(existingDocuments);

            var handler = new GenerateSupplierImportDocumentCodeQueryHandler(mockDbContext.Object);
            var query = new GenerateSupplierImportDocumentCodeQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{codePrefix}-002", result.DocumentCode);
            Assert.Equal($"{yearPrefix}003", result.DocumentNumber);
        }

        [Fact]
        public async Task Handle_WithInvalidFormatDocuments_ShouldHandleGracefully()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            var codePrefix = $"PN{dateString}";
            var yearPrefix = $"NK{today.Year}_";

            var existingDocuments = new List<SupplierImportDocument>
            {
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-001", DocumentNumber = $"{yearPrefix}001" },
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-invalid", DocumentNumber = "invalid" },
                new SupplierImportDocument { DocumentCode = $"{codePrefix}-002", DocumentNumber = $"{yearPrefix}002" },
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(x => x.SupplierImportDocuments)
                .ReturnsDbSet(existingDocuments);

            var handler = new GenerateSupplierImportDocumentCodeQueryHandler(mockDbContext.Object);
            var query = new GenerateSupplierImportDocumentCodeQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{codePrefix}-003", result.DocumentCode);
            Assert.Equal($"{yearPrefix}003", result.DocumentNumber);
        }

        [Fact]
        public async Task GenerateSupplierImportDocumentCode_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/supplier-import-documents/generate-code");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GenerateSupplierImportDocumentCodeResponse>();
            result.Should().NotBeNull();
            result!.DocumentCode.Should().NotBeNullOrEmpty();
            result.DocumentNumber.Should().NotBeNullOrEmpty();

            // Verify format: PN[YYYYMMDD]-[Sequence]
            var today = DateTime.Now;
            string dateString = $"{today.Year}{today.Month:D2}{today.Day:D2}";
            result.DocumentCode.Should().StartWith($"PN{dateString}");
            result.DocumentCode.Should().MatchRegex(@"^PN\d{8}-\d{3}$");

            // Verify format: NK[YYYY]_[Sequence]
            result.DocumentNumber.Should().StartWith($"NK{today.Year}_");
            result.DocumentNumber.Should().MatchRegex(@"^NK\d{4}_\d{3}$");
        }

        [Fact]
        public async Task GenerateSupplierImportDocumentCode_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/supplier-import-documents/generate-code");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}