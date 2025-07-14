using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Medicines.Commands.CreateMedicinePrice;
using Inventory.Application.Medicines.Commands.DeleteMedicinePrice;
using Inventory.Application.Medicines.Commands.UpdateMedicinePrice;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class MedicinePriceEndpointsTests : BaseFunctionalTest
    {
        public MedicinePriceEndpointsTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        #region GetMedicinePrices

        [Fact]
        public async Task GetMedicinePrices_WithValidData_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/medicine-prices");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinePricesResponse>();
            result.Should().NotBeNull();
            result.MedicinePrices.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicinePrices_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/medicine-prices");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicinePrices_WithInvalidPagination_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/medicine-prices?pageIndex=-1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Case where no medicine id exist
        [Fact]
        public async Task GetMedicinePrices_WithNonExistentMedicineId_ReturnsNotFound()
        {
            // Arrange
            var medicineId = 9999; // Assuming this ID does not exist
            // Act
            var response = await _client.GetAsync($"/medicine-prices/{medicineId}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case where no medicine price with medicine id exist
        [Fact]
        public async Task GetMedicinePrices_WithNonExistentMedicinePrice_ReturnsNotFound()
        {
            // Arrange
            var medicineId = 6; // Assuming this ID exists but has no prices
            // Act
            var response = await _client.GetAsync($"/medicine-prices/{medicineId}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region CreateMedicinePrice

        [Fact]
        public async Task CreateMedicinePrice_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateMedicinePriceCommand(
                MedicineId: 1,
                UnitPrice: 10.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.05m,
                OriginalPriceAfterVat: 11.55m,
                OriginalPriceBeforeVat: 10.5m
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-prices", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateMedicinePriceResponse>();
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateMedicinePrice_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateMedicinePriceCommand(
                MedicineId: 1,
                UnitPrice: 10.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.05m,
                OriginalPriceAfterVat: 11.55m,
                OriginalPriceBeforeVat: 10.5m
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-prices", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateMedicinePrice_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicinePriceCommand(
                MedicineId: 0, // Invalid ID
                UnitPrice: -1, // Negative price
                Currency: "",
                VatRate: -1.0,
                VatAmount: -1,
                OriginalPriceAfterVat: -1,
                OriginalPriceBeforeVat: -1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicine-prices", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateMedicinePrice_WithNonExistentMedicineId_ReturnsNotFound()
        {
            // Arrange
            var command = new CreateMedicinePriceCommand(
                MedicineId: 9999, // Assuming this ID does not exist
                UnitPrice: 10.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.05m,
                OriginalPriceAfterVat: 11.55m,
                OriginalPriceBeforeVat: 10.5m
            );
            // Act
            var response = await _client.PostAsJsonAsync("/medicine-prices", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region UpdateMedicinePrice

        [Fact]
        public async Task UpdateMedicinePrice_WithValidData_ReturnsOk()
        {
            // Arrange
            // First, create a medicine price to update
            var createCommand = new CreateMedicinePriceCommand(
                MedicineId: 1,
                UnitPrice: 10.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.05m,
                OriginalPriceAfterVat: 11.55m,
                OriginalPriceBeforeVat: 10.5m
            );

            var createResponse = await _client.PostAsJsonAsync("/medicine-prices", createCommand);
            createResponse.EnsureSuccessStatusCode();
            var createResult = await createResponse.Content.ReadFromJsonAsync<CreateMedicinePriceResponse>();
            var id = createResult!.Id;

            var updateCommand = new UpdateMedicinePriceCommand(
                Id: id,
                MedicineId: 1,
                UnitPrice: 12.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.25m,
                OriginalPriceAfterVat: 13.75m,
                OriginalPriceBeforeVat: 12.5m
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-prices/{id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateMedicinePriceResult>();
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateMedicinePrice_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var id = 1;
            var command = new UpdateMedicinePriceCommand(
                Id: id,
                MedicineId: 1,
                UnitPrice: 12.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.25m,
                OriginalPriceAfterVat: 13.75m,
                OriginalPriceBeforeVat: 12.5m
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-prices/{id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateMedicinePrice_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var command = new UpdateMedicinePriceCommand(
                Id: 2, // Different from URL ID
                MedicineId: 1,
                UnitPrice: 12.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.25m,
                OriginalPriceAfterVat: 13.75m,
                OriginalPriceBeforeVat: 12.5m
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-prices/{id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Case where medicine price does not exist
        [Fact]
        public async Task UpdateMedicinePrice_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var id = 9999; // Assuming this ID does not exist
            var command = new UpdateMedicinePriceCommand(
                Id: id,
                MedicineId: 1,
                UnitPrice: 12.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.25m,
                OriginalPriceAfterVat: 13.75m,
                OriginalPriceBeforeVat: 12.5m
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-prices/{id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case where no medicine price exist
        [Fact]
        public async Task UpdateMedicinePrice_WithNonExistentMedicinePriceId_ReturnsNotFound()
        {
            var id = 99999; // Assuming this ID does not exist
            // Arrange
            var command = new UpdateMedicinePriceCommand(
                Id: id,
                MedicineId: 1,
                UnitPrice: 12.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.25m,
                OriginalPriceAfterVat: 13.75m,
                OriginalPriceBeforeVat: 12.5m
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-prices/{id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        #endregion

        #region DeleteMedicinePrice

        [Fact]
        public async Task DeleteMedicinePrice_WithValidId_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var response = await _client.DeleteAsync($"/medicine-prices/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DeleteMedicinePriceResult>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteMedicinePrice_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var id = 1;

            // Act
            var response = await _client.DeleteAsync($"/medicine-prices/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteMedicinePrice_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var id = -1; // Invalid ID

            // Act
            var response = await _client.DeleteAsync($"/medicine-prices/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Case where the ID does not exist
        [Fact]
        public async Task DeleteMedicinePrice_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var id = 9999; // Assuming this ID does not exist
            // Act
            var response = await _client.DeleteAsync($"/medicine-prices/{id}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region GetMedicinePricesByMedicineId

        [Fact]
        public async Task GetMedicinePricesByMedicineId_WithValidId_ReturnsOk()
        {
            // Arrange
            // First, create a medicine price for our medicine ID
            var medicineId = 1;
            var createCommand = new CreateMedicinePriceCommand(
                MedicineId: medicineId,
                UnitPrice: 10.5m,
                Currency: "USD",
                VatRate: 10.0,
                VatAmount: 1.05m,
                OriginalPriceAfterVat: 11.55m,
                OriginalPriceBeforeVat: 10.5m
            );

            await _client.PostAsJsonAsync("/medicine-prices", createCommand);

            // Act 
            var response = await _client.GetAsync($"/medicine-prices/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinePricesByMedicineIdResponse>();
            result.Should().NotBeNull();
            result.MedicinePrices.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicinePricesByMedicineId_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var medicineId = 1;

            // Act
            var response = await _client.GetAsync($"/medicine-prices/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicinePricesByMedicineId_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var medicineId = -1; // Invalid ID

            // Act
            var response = await _client.GetAsync($"/medicine-prices/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion
    }
}
