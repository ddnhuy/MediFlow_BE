using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Quartz;
using VaccinationReception.Application.Jobs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;
using VaccinationReception.Application.Data;
using MockQueryable.Moq;
using System;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CleanupUnpaidItemsJobTests
    {
        [Fact]
        public async Task Execute_ShouldCancelUnpaidItemsPaymentsAndSaveChanges()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CleanupUnpaidItemsJob>>();

            // Mock data
            var receptionId = 1;
            var paymentId = 100;
            var paymentDetailId1 = 200;
            var paymentDetailId2 = 201;

            var cutoffTime = DateTime.UtcNow.AddHours(-5);
            var cutoffLatestActivityTime = DateTime.UtcNow.AddHours(-2);

            var receptions = new List<Reception>
            {
                new Reception
                {
                    Id = receptionId,
                    ReceptionDate = cutoffTime.AddHours(-1), // Older than cutoff
                    LastUpdatedAt = cutoffLatestActivityTime.AddHours(-1) // Older than cutoff
                }
            }.AsQueryable();

            var receptionVaccinations = new List<ReceptionVaccination>
            {
                new ReceptionVaccination
                {
                    Id = 1,
                    ReceptionId = receptionId,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    IsCancelled = false
                }
            }.AsQueryable();

            var serviceRequestDetails = new List<ServiceRequestDetail>
            {
                new ServiceRequestDetail
                {
                    Id = 2,
                    ReceptionId = receptionId,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    IsCancelled = false
                }
            }.AsQueryable();

            var payments = new List<Payment>
            {
                new Payment
                {
                    Id = paymentId,
                    ReceptionId = receptionId,
                    Status = PaymentStatus.Pending,
                    LastUpdatedAt = cutoffLatestActivityTime.AddHours(-1), // Older than cutoff
                    IsCancelled = false
                }
            }.AsQueryable();

            var paymentDetails = new List<PaymentDetail>
            {
                new PaymentDetail
                {
                    Id = paymentDetailId1,
                    PaymentId = paymentId,
                    Amount = 100,
                    IsCancelled = false
                },
                new PaymentDetail
                {
                    Id = paymentDetailId2,
                    PaymentId = paymentId,
                    Amount = 50,
                    IsCancelled = false
                }
            }.AsQueryable();

            // Mock DbSet
            var receptionsDbSet = receptions.BuildMockDbSet();
            var receptionVaccinationsDbSet = receptionVaccinations.BuildMockDbSet();
            var serviceRequestDetailsDbSet = serviceRequestDetails.BuildMockDbSet();
            var paymentsDbSet = payments.BuildMockDbSet();
            var paymentDetailsDbSet = paymentDetails.BuildMockDbSet();

            var dbContextMock = new Mock<IApplicationDbContext>();
            dbContextMock.Setup(x => x.Receptions).Returns(receptionsDbSet.Object);
            dbContextMock.Setup(x => x.ReceptionVaccinations).Returns(receptionVaccinationsDbSet.Object);
            dbContextMock.Setup(x => x.ServiceRequestDetails).Returns(serviceRequestDetailsDbSet.Object);
            dbContextMock.Setup(x => x.Payments).Returns(paymentsDbSet.Object);
            dbContextMock.Setup(x => x.PaymentDetails).Returns(paymentDetailsDbSet.Object);

            // Setup SaveChangesAsync to return total number of updated entities
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5); // 1 reception vaccination + 1 service detail + 1 payment + 2 payment details

            var job = new CleanupUnpaidItemsJob(loggerMock.Object, dbContextMock.Object);

            // Act
            await job.Execute(Mock.Of<IJobExecutionContext>());

            // Assert
            // Verify ReceptionVaccinations are cancelled
            Assert.True(receptionVaccinations.First().IsCancelled);

            // Verify ServiceRequestDetails are cancelled
            Assert.True(serviceRequestDetails.First().IsCancelled);

            // Verify Payments are cancelled
            Assert.True(payments.First().IsCancelled);

            // Verify PaymentDetails are cancelled
            Assert.True(paymentDetails.First().IsCancelled);
            Assert.True(paymentDetails.Last().IsCancelled);

            // Verify SaveChangesAsync was called once
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}