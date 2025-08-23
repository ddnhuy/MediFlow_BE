using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Linq;
using FluentAssertions;
using HospitalFee.FunctionalTests.Abstractions;
using HospitalFee.FunctionalTests.Helpers;
using NSubstitute;
using VaccinationReception.API.EndPoints.HospitalFeeEndpoints;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalFee.FunctionalTests.Tests
{
    public class GetAllPaymentsWithPatientsEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetAllPaymentsWithPatientsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        public Task InitializeAsync() => _factory.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllPaymentsWithPatients_WithoutToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/payments?pageIndex=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllPaymentsWithPatients_WithData_ReturnsOkAndIncludesPatients()
        {
            SetAuthHeader();

            var reception1 = new Reception { PatientId = 1, ServiceTypeId = 1 };
            var reception2 = new Reception { PatientId = 2, ServiceTypeId = 1 };
            await SeedEntityAsync(reception1);
            await SeedEntityAsync(reception2);

            var payment1 = new Payment
            {
                ReceptionId = reception1.Id,
                TotalAmount = 100m,
                Method = PaymentMethod.Cash,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-001",
                Status = PaymentStatus.Pending
            };
            var payment2 = new Payment
            {
                ReceptionId = reception2.Id,
                TotalAmount = 200m,
                Method = PaymentMethod.CreditCard,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-002",
                Status = PaymentStatus.Completed
            };
            await SeedEntityAsync(payment1);
            await SeedEntityAsync(payment2);

            _factory.PatientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(new List<PatientSummaryDTO>
                {
                    new PatientSummaryDTO { Id = 1, Name = "Alice", Code = "P001", IdentityCard = "ID123" },
                    new PatientSummaryDTO { Id = 2, Name = "Bob",   Code = "P002", IdentityCard = "ID456" }
                });

            var response = await _client.GetAsync("/payments?pageIndex=1&pageSize=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<GetAllPaymentsWithPatientsResponse>();
            content.Should().NotBeNull();
            content!.Payments.Should().NotBeNull();
            content.Payments.Data.Should().HaveCount(2);

            content.Payments.Data.Should().Contain(item => item.Patient.Id == 1 && item.Payment.InvoiceNumber == "INV-001");
            content.Payments.Data.Should().Contain(item => item.Patient.Id == 2 && item.Payment.InvoiceNumber == "INV-002");
        }

        [Fact]
        public async Task GetAllPaymentsWithPatients_Search_FiltersByPatientAndInvoice()
        {
            SetAuthHeader();
            await _factory.ResetDatabaseAsync();
            _dbContext.ChangeTracker.Clear();

            var reception1 = new Reception { PatientId = 1, ServiceTypeId = 1 };
            var reception2 = new Reception { PatientId = 2, ServiceTypeId = 1 };
            _dbContext.Receptions.AddRange(reception1, reception2);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();

            var payment1 = new Payment
            {
                ReceptionId = reception1.Id,
                TotalAmount = 100m,
                Method = PaymentMethod.Cash,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-111",
                Status = PaymentStatus.Pending
            };
            var payment2 = new Payment
            {
                ReceptionId = reception2.Id,
                TotalAmount = 200m,
                Method = PaymentMethod.CreditCard,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-222",
                Status = PaymentStatus.Completed
            };
            await SeedEntityAsync(payment1);
            await SeedEntityAsync(payment2);

            _factory.PatientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(new List<PatientSummaryDTO>
                {
                    new PatientSummaryDTO { Id = 1, Name = "Alice", Code = "P001", IdentityCard = "ID123" },
                    new PatientSummaryDTO { Id = 2, Name = "Bob",   Code = "P002", IdentityCard = "ID456" }
                });

            // Search by Name
            var byName = await _client.GetAsync("/payments?pageIndex=1&pageSize=10&searchTerm=Alice");
            var byNameContent = await byName.Content.ReadFromJsonAsync<GetAllPaymentsWithPatientsResponse>();
            byName.StatusCode.Should().Be(HttpStatusCode.OK);
            byNameContent!.Payments.Data.Should().HaveCount(1);
            byNameContent.Payments.Data.First().Patient.Name.Should().Be("Alice");

            // Search by Code
            var byCode = await _client.GetAsync("/payments?pageIndex=1&pageSize=10&searchTerm=P002");
            var byCodeContent = await byCode.Content.ReadFromJsonAsync<GetAllPaymentsWithPatientsResponse>();
            byCode.StatusCode.Should().Be(HttpStatusCode.OK);
            byCodeContent!.Payments.Data.Should().HaveCount(1);
            byCodeContent.Payments.Data.First().Patient.Code.Should().Be("P002");

            // Search by IdentityCard
            var byIdentity = await _client.GetAsync("/payments?pageIndex=1&pageSize=10&searchTerm=ID123");
            var byIdentityContent = await byIdentity.Content.ReadFromJsonAsync<GetAllPaymentsWithPatientsResponse>();
            byIdentity.StatusCode.Should().Be(HttpStatusCode.OK);
            byIdentityContent!.Payments.Data.Should().HaveCount(1);
            byIdentityContent.Payments.Data.First().Patient.IdentityCard.Should().Be("ID123");

            // Search by InvoiceNumber
            var byInvoice = await _client.GetAsync("/payments?pageIndex=1&pageSize=10&searchTerm=INV-222");
            var byInvoiceContent = await byInvoice.Content.ReadFromJsonAsync<GetAllPaymentsWithPatientsResponse>();
            byInvoice.StatusCode.Should().Be(HttpStatusCode.OK);
            byInvoiceContent!.Payments.Data.Should().HaveCount(1);
            byInvoiceContent.Payments.Data.First().Payment.InvoiceNumber.Should().Be("INV-222");
        }

        [Fact]
        public async Task GetAllPaymentsWithPatients_With_From_And_To_Date()
        {
            SetAuthHeader();
            await _factory.ResetDatabaseAsync();
            _dbContext.ChangeTracker.Clear();

            var reception1 = new Reception { PatientId = 1, ServiceTypeId = 1 };
            var reception2 = new Reception { PatientId = 2, ServiceTypeId = 1 };
            _dbContext.Receptions.AddRange(reception1, reception2);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();

            var payment1 = new Payment
            {
                ReceptionId = reception1.Id,
                TotalAmount = 100m,
                Method = PaymentMethod.Cash,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-212",
                Status = PaymentStatus.Pending
            };
            var payment2 = new Payment
            {
                ReceptionId = reception2.Id,
                TotalAmount = 200m,
                Method = PaymentMethod.CreditCard,
                PaymentType = PaymentType.Receipt,
                InvoiceNumber = "INV-123",
                Status = PaymentStatus.Completed
            };
            await SeedEntityAsync(payment1);
            await SeedEntityAsync(payment2);

            _factory.PatientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(new List<PatientSummaryDTO>
                {
                    new PatientSummaryDTO { Id = 1, Name = "Alice", Code = "P001", IdentityCard = "ID123" },
                    new PatientSummaryDTO { Id = 2, Name = "Bob",   Code = "P002", IdentityCard = "ID456" }
                });

            var response = await _client.GetAsync("/payments?pageIndex=1&pageSize=10&fromDate=2025-01-12&toDate=2025-02-01");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}