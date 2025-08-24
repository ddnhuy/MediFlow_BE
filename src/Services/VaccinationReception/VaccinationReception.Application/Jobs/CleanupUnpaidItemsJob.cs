using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.Jobs
{
    public class CleanupUnpaidItemsJob : IJob
    {
        private readonly ILogger<CleanupUnpaidItemsJob> _logger;
        private readonly IApplicationDbContext _context;

        public CleanupUnpaidItemsJob(ILogger<CleanupUnpaidItemsJob> logger, IApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting cleanup job at: {DateTime}", DateTime.UtcNow);

            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-5);
                var cutoffLatestActivityTime = DateTime.UtcNow.AddHours(-2);
                _logger.LogInformation("Cutoff time: {CutoffTime}", cutoffTime);

                var oldReceptions = await _context.Receptions
                    .Where(r => r.ReceptionDate <= cutoffTime && r.LastUpdatedAt <= cutoffLatestActivityTime)
                    .Select(r => r.Id)
                    .ToListAsync();

                if (!oldReceptions.Any())
                {
                    _logger.LogInformation("No Receptions found to clean up.");
                    return;
                }

                _logger.LogInformation("Found {Count} Receptions to clean up.", oldReceptions.Count);

                var receptionVaccinationsToUpdate = await _context.ReceptionVaccinations
                    .Where(rv => oldReceptions.Contains(rv.ReceptionId) &&
                                 rv.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync();

                var vaccinationCount = receptionVaccinationsToUpdate.Count;

                if (vaccinationCount > 0)
                {
                    foreach (var rv in receptionVaccinationsToUpdate)
                    {
                        rv.IsCancelled = true;
                    }

                    _logger.LogInformation("Will cancel {Count} ReceptionVaccination records.", vaccinationCount);
                }

                var serviceRequestDetailsToUpdate = await _context.ServiceRequestDetails
                    .Where(srd => oldReceptions.Contains(srd.ReceptionId) &&
                                  srd.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync();

                var serviceDetailCount = serviceRequestDetailsToUpdate.Count;

                if (serviceDetailCount > 0)
                {
                    foreach (var srd in serviceRequestDetailsToUpdate)
                    {
                        srd.IsCancelled = true;
                    }

                    _logger.LogInformation("Will cancel {Count} ServiceRequestDetail records.", serviceDetailCount);
                }

                var pendingPaymentsToCancel = await _context.Payments
                    .Where(p => p.LastUpdatedAt <= cutoffLatestActivityTime &&
                                p.Status == PaymentStatus.Pending)
                    .ToListAsync();

                var paymentCount = pendingPaymentsToCancel.Count;
                if (paymentCount > 0)
                {
                    foreach (var payment in pendingPaymentsToCancel)
                    {
                        payment.IsCancelled = true;
                    }
                    _logger.LogInformation("Will cancel {Count} Pending Payment records.", paymentCount);
                }

                if (paymentCount > 0)
                {
                    var paymentIds = pendingPaymentsToCancel.Select(p => p.Id).ToList();
                    var paymentDetailsToCancel = await _context.PaymentDetails
                        .Where(pd => paymentIds.Contains(pd.PaymentId))
                        .ToListAsync();

                    var paymentDetailCount = paymentDetailsToCancel.Count;
                    if (paymentDetailCount > 0)
                    {
                        foreach (var paymentDetail in paymentDetailsToCancel)
                        {
                            paymentDetail.IsCancelled = true;
                        }
                        _logger.LogInformation("Will cancel {Count} PaymentDetail records.", paymentDetailCount);
                    }
                }

                var totalUpdated = await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated {Count} records.", totalUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing the cleanup job.");
                throw;
            }
        }
    }
}