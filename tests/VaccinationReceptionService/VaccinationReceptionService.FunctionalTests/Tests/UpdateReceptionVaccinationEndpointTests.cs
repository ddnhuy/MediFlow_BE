using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdateReceptionVaccinationEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionVaccinationId = 1;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;

        public UpdateReceptionVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        public async Task InitializeAsync()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Reception if not exists
            var reception = await dbContext.Receptions.FirstOrDefaultAsync(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = 1,
                    ReceptionDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.Receptions.AddAsync(reception);
            }
            
            // Create ReceptionVaccination if not exists
            var receptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = false,
                    ScheduledDate = DateTime.Now,
                    InvoiceDate = DateTime.Now,
                    AppointmentDate = DateTime.Now,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    IsConfirmed = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.ReceptionVaccinations.AddAsync(receptionVaccination);
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{TestReceptionVaccinationId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithValidData_ReturnsOk()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{TestReceptionVaccinationId}", command);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateReceptionVaccinationResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var receptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            receptionVaccination.Should().NotBeNull();
            receptionVaccination!.Quantity.Should().Be(command.Quantity);
            receptionVaccination.IsReadyToUse.Should().Be(command.IsReadyToUse);

            receptionVaccination.ScheduledDate.Should().BeCloseTo(command.ScheduledDate, TimeSpan.FromSeconds(1));
            receptionVaccination.InvoiceDate.Should().BeCloseTo(command.InvoiceDate, TimeSpan.FromSeconds(1));
            receptionVaccination.AppointmentDate.Should().BeCloseTo(command.AppointmentDate, TimeSpan.FromSeconds(1));

            receptionVaccination.PaymentStatus.Should().Be(command.PaymentStatus);
            receptionVaccination.IsConfirmed.Should().Be(command.IsConfirmed);
            receptionVaccination.Note.Should().Be(command.Note);
            receptionVaccination.TestResultEntry.Should().Be(command.TestResultEntry);
            receptionVaccination.DoctorId.Should().Be(command.DoctorId);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var command = CreateValidCommand();
            var differentId = TestReceptionVaccinationId + 1;

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{differentId}", command);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private UpdateReceptionVaccinationCommand CreateValidCommand()
        {
            return new UpdateReceptionVaccinationCommand(
                Id: TestReceptionVaccinationId,
                Quantity: 2,
                IsReadyToUse: true,
                ScheduledDate: DateTime.Now.AddDays(1),
                InvoiceDate: DateTime.Now,
                AppointmentDate: DateTime.Now.AddDays(2),
                PaymentStatus: PaymentStatusForItem.Paid,
                IsConfirmed: true,
                Note: "Test note",
                TestResultEntry: "Test result",
                DoctorId: TestDoctorId
            );
        }
    }
}