using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class ConfirmVaccinationTodayEndpointTests : BaseFunctionalTest
    {
        private const int TestReceptionId = 1001;
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public ConfirmVaccinationTodayEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task ConfirmVaccinationToday_Success_WhenAllConditionsMet()
        {
            // Arrange: Tạo Reception, ReceptionVaccination, Vaccination thỏa mãn điều kiện
            var dbContext = GetDbContext();

            var reception = new Reception
            {
                Id = TestReceptionId,
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow,
                ServiceTypeId = 1,
                IsVaccinationTodayConfirmed = false
            };
            dbContext.Receptions.Add(reception);

            var rv = new ReceptionVaccination
            {
                Id = 2001,
                ReceptionId = TestReceptionId,
                VaccineId = 1,
                Quantity = 1,
                RequestNumber = "REQ-001",
                //IsConfirmed = true
            };
            dbContext.ReceptionVaccinations.Add(rv);

            var vaccination = new Vaccination
            {
                ReceptionVaccinationId = rv.Id,
                PatientId = 1,
                ObservationConfirmed = true,
                IsConfirmed = true
            };
            dbContext.Vaccinations.Add(vaccination);

            await dbContext.SaveChangesAsync();

            // Act
            var response = await _client.PutAsync($"vaccination/receptions/{TestReceptionId}/confirm-vaccination-today", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ConfirmVaccinationTodayResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Kiểm tra DB đã cập nhật
            var updatedReception = await dbContext.Receptions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == TestReceptionId);
        }

        [Fact]
        public async Task ConfirmVaccinationToday_Fail_WhenNotAllReceptionVaccinationConfirmed()
        {
            // Arrange
            var dbContext = GetDbContext();

            var reception = new Reception
            {
                Id = TestReceptionId + 1,
                PatientId = 2,
                ReceptionDate = DateTime.UtcNow,
                ServiceTypeId = 1,
                IsVaccinationTodayConfirmed = false
            };
            dbContext.Receptions.Add(reception);

            var rv = new ReceptionVaccination
            {
                Id = 2002,
                ReceptionId = reception.Id,
                VaccineId = 1,
                Quantity = 1,
                RequestNumber = "REQ-002",
                //IsConfirmed = false // Chưa xác nhận
            };
            dbContext.ReceptionVaccinations.Add(rv);

            var vaccination = new Vaccination
            {
                ReceptionVaccinationId = rv.Id,
                PatientId = 2,
                ObservationConfirmed = true
            };
            dbContext.Vaccinations.Add(vaccination);

            await dbContext.SaveChangesAsync();

            // Act
            var response = await _client.PutAsync($"vaccination/receptions/{TestReceptionId}/confirm-vaccination-today", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ConfirmVaccinationToday_Fail_WhenNotAllVaccinationsObserved()
        {
            // Arrange
            var dbContext = GetDbContext();

            var reception = new Reception
            {
                Id = TestReceptionId + 2,
                PatientId = 3,
                ReceptionDate = DateTime.UtcNow,
                ServiceTypeId = 1,
                IsVaccinationTodayConfirmed = false
            };
            dbContext.Receptions.Add(reception);

            var rv = new ReceptionVaccination
            {
                Id = 2003,
                ReceptionId = reception.Id,
                VaccineId = 1,
                Quantity = 1,
                RequestNumber = "REQ-003",
                //IsConfirmed = true
            };
            dbContext.ReceptionVaccinations.Add(rv);

            var vaccination = new Vaccination
            {
                ReceptionVaccinationId = rv.Id,
                PatientId = 3,
                ObservationConfirmed = false // Chưa theo dõi sau tiêm
            };
            dbContext.Vaccinations.Add(vaccination);

            await dbContext.SaveChangesAsync();

            // Act
            var response = await _client.PutAsync($"vaccination/receptions/{TestReceptionId}/confirm-vaccination-today", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}
