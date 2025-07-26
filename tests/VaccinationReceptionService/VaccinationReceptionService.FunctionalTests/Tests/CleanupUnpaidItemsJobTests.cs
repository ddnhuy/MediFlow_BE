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

public class CleanupUnpaidItemsJobTests
{
    //[Fact]
    //public async Task Execute_ShouldCancelUnpaidItemsAndSaveChanges()
    //{
    //    // Arrange
    //    var loggerMock = new Mock<ILogger<CleanupUnpaidItemsJob>>();

    //    // Mock data
    //    var receptionId = 1;
    //    var receptions = new List<Reception>
    //    {
    //        new Reception { Id = receptionId, ReceptionDate = DateTime.UtcNow.AddHours(-6) }
    //    }.AsQueryable();

    //    var receptionVaccinations = new List<ReceptionVaccination>
    //    {
    //        new ReceptionVaccination { ReceptionId = receptionId, PaymentStatus = PaymentStatusForItem.NotPaid, IsCancelled = false }
    //    }.AsQueryable();

    //    var serviceRequestDetails = new List<ServiceRequestDetail>
    //    {
    //        new ServiceRequestDetail { ReceptionId = receptionId, PaymentStatus = PaymentStatusForItem.NotPaid, IsCancelled = false }
    //    }.AsQueryable();

    //    // Mock DbSet
    //    var receptionsDbSet = receptions.BuildMockDbSet();
    //    var receptionVaccinationsDbSet = receptionVaccinations.BuildMockDbSet();
    //    var serviceRequestDetailsDbSet = serviceRequestDetails.BuildMockDbSet();

    //    var dbContextMock = new Mock<IApplicationDbContext>();
    //    dbContextMock.Setup(x => x.Receptions).Returns(receptionsDbSet.Object);
    //    dbContextMock.Setup(x => x.ReceptionVaccinations).Returns(receptionVaccinationsDbSet.Object);
    //    dbContextMock.Setup(x => x.ServiceRequestDetails).Returns(serviceRequestDetailsDbSet.Object);
    //    dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

    //    var job = new CleanupUnpaidItemsJob(loggerMock.Object, dbContextMock.Object);

    //    // Act
    //    await job.Execute(Mock.Of<IJobExecutionContext>());

    //    // Assert
    //    Assert.True(receptionVaccinations.First().IsCancelled);
    //    Assert.True(serviceRequestDetails.First().IsCancelled);
    //    dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    //}
}