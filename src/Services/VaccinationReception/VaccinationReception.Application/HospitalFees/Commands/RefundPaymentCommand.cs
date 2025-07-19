using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.HospitalFees.Commands
{
    public record RefundPaymentCommand(
    int ReceptionId,
    int OriginalPaymentId,
    PaymentMethod Method,
    string? Note,
    List<int> RefundedReceptionVaccinationIds,
    List<int> RefundedServiceRequestDetailIds) : ICommand<RefundPaymentResult>;

    public record RefundPaymentResult(int RefundPaymentId);
    public class RefundPaymentCommandHandler : ICommandHandler<RefundPaymentCommand, RefundPaymentResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<RefundPaymentCommandHandler> _logger;

        public RefundPaymentCommandHandler(IApplicationDbContext dbContext, ILogger<RefundPaymentCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<RefundPaymentResult> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin RefundPaymentCommand for OriginalPaymentId: {OriginalPaymentId}", request.OriginalPaymentId);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var originalPayment = await _dbContext.Payments
                    .FirstOrDefaultAsync(p => p.Id == request.OriginalPaymentId && p.ReceptionId == request.ReceptionId, cancellationToken);

                if (originalPayment == null)
                    throw new NotFoundException(ExceptionKey.ORIGINAL_PAYMENT_NOT_FOUND_FOR_RECEPTION);

                var itemsToRefundVaccination = await _dbContext.ReceptionVaccinations
                    .Where(rv => request.RefundedReceptionVaccinationIds.Contains(rv.Id) && rv.ReceptionId == request.ReceptionId && rv.PaymentStatus == PaymentStatusForItem.Paid)
                    .ToListAsync(cancellationToken);

                var itemsToRefundService = await _dbContext.ServiceRequestDetails
                    .Where(sd => request.RefundedServiceRequestDetailIds.Contains(sd.Id) && sd.ReceptionId == request.ReceptionId && sd.PaymentStatus == PaymentStatusForItem.Paid)
                    .ToListAsync(cancellationToken);

                if (itemsToRefundVaccination.Count != (request.RefundedReceptionVaccinationIds?.Count ?? 0) ||
                    itemsToRefundService.Count != (request.RefundedServiceRequestDetailIds?.Count ?? 0))
                {
                    throw new BadRequestException(ExceptionKey.REFUNDED_ITEMS_NOT_PAID_OR_INVALID);
                }

                decimal totalRefundAmount = 0;
                var refundDetails = new List<PaymentDetail>();

                foreach (var item in itemsToRefundVaccination)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalRefundAmount -= amount;
                    refundDetails.Add(new PaymentDetail { ReceptionVaccinationId = item.Id, Amount = -amount });
                    item.PaymentStatus = PaymentStatusForItem.Refunded;
                }

                foreach (var item in itemsToRefundService)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalRefundAmount -= amount;
                    refundDetails.Add(new PaymentDetail { ServiceRequestDetailId = item.Id, Amount = -amount });
                    item.PaymentStatus = PaymentStatusForItem.Refunded;
                }

                if (totalRefundAmount >= 0)
                    throw new BadRequestException(ExceptionKey.REFUND_AMOUNT_MUST_BE_NEGATIVE);

                var refundPayment = new Payment
                {
                    ReceptionId = request.ReceptionId,
                    TotalAmount = totalRefundAmount,
                    Method = request.Method,
                    Note = request.Note,
                    InvoiceNumber = UniqueStringGenerator.GenerateInvoiceNumber(),
                    PaymentType = PaymentType.Refund,
                    Status = PaymentStatus.Completed,
                    OriginalPaymentId = request.OriginalPaymentId,
                    PaymentDetails = refundDetails
                };

                _dbContext.Payments.Add(refundPayment);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
        
                _logger.LogInformation("Successfully created Refund Payment with Id: {RefundPaymentId}", refundPayment.Id);
                return new RefundPaymentResult(refundPayment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during RefundPaymentCommand for OriginalPaymentId: {OriginalPaymentId}", request.OriginalPaymentId);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
