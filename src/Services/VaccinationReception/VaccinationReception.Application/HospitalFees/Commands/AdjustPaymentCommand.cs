using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.HospitalFees.Commands
{
    public record AdjustPaymentCommand(
        int ReceptionId,
        int OriginalPaymentId,
        string Method,
        string? Note,
        List<int> CancelledReceptionVaccinationIds,
        List<int> CancelledServiceRequestDetailIds,
        List<int> NewReceptionVaccinationIds,
        List<int> NewServiceRequestDetailIds) : ICommand<AdjustPaymentResult>;

    public record AdjustPaymentResult(int AdjustmentPaymentId);
    public class AdjustPaymentCommandHandler : ICommandHandler<AdjustPaymentCommand, AdjustPaymentResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AdjustPaymentCommandHandler> _logger;

        public AdjustPaymentCommandHandler(IApplicationDbContext context, ILogger<AdjustPaymentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AdjustPaymentResult> Handle(AdjustPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin AdjustPaymentCommand for OriginalPaymentId: {OriginalPaymentId}", request.OriginalPaymentId);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var originalPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.Id == request.OriginalPaymentId && p.ReceptionId == request.ReceptionId, cancellationToken);

                if (originalPayment == null)
                {
                    throw new NotFoundException(ExceptionKey.ORIGINAL_PAYMENT_NOT_FOUND_FOR_RECEPTION);
                }

                var itemsToCancelVaccination = await _context.ReceptionVaccinations
                    .Where(x => request.CancelledReceptionVaccinationIds.Contains(x.Id)
                             && x.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.Paid)
                    .ToListAsync(cancellationToken);

                var itemsToCancelService = await _context.ServiceRequestDetails
                    .Include(x => x.RequestForm)
                    .Where(x => request.CancelledServiceRequestDetailIds.Contains(x.Id)
                             && x.RequestForm.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.Paid)
                    .ToListAsync(cancellationToken);

                var itemsToAddVaccination = await _context.ReceptionVaccinations
                    .Where(x => request.NewReceptionVaccinationIds.Contains(x.Id)
                             && x.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync(cancellationToken);

                var itemsToAddService = await _context.ServiceRequestDetails
                    .Include(x => x.RequestForm)
                    .Where(x => request.NewServiceRequestDetailIds.Contains(x.Id)
                             && x.RequestForm.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync(cancellationToken);

                if (itemsToCancelVaccination.Count != request.CancelledReceptionVaccinationIds.Count ||
                    itemsToCancelService.Count != request.CancelledServiceRequestDetailIds.Count)
                {
                    throw new BadRequestException(ExceptionKey.CANCEL_ITEMS_NOT_PAID_OR_INVALID);
                }

                if (itemsToAddVaccination.Count != request.NewReceptionVaccinationIds.Count ||
                    itemsToAddService.Count != request.NewServiceRequestDetailIds.Count)
                {
                    throw new BadRequestException(ExceptionKey.ONE_OR_MORE_ITEMS_ALREADY_PAID_OR_INVALID);
                }

                decimal totalAmountDifference = 0;
                var adjustmentDetails = new List<PaymentDetail>();

                foreach (var item in itemsToCancelVaccination)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalAmountDifference -= amount;
                    adjustmentDetails.Add(new PaymentDetail { ReceptionVaccinationId = item.Id, Amount = -amount });
                    item.PaymentStatus = PaymentStatusForItem.AdjustedOut;
                }

                foreach (var item in itemsToCancelService)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalAmountDifference -= amount;
                    adjustmentDetails.Add(new PaymentDetail { ServiceRequestDetailId = item.Id, Amount = -amount });
                    item.PaymentStatus = PaymentStatusForItem.AdjustedOut;
                }

                foreach (var item in itemsToAddVaccination)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalAmountDifference += amount;
                    adjustmentDetails.Add(new PaymentDetail { ReceptionVaccinationId = item.Id, Amount = amount });
                    item.PaymentStatus = PaymentStatusForItem.Paid;
                }

                foreach (var item in itemsToAddService)
                {
                    var amount = item.UnitPrice * item.Quantity;
                    totalAmountDifference += amount;
                    adjustmentDetails.Add(new PaymentDetail { ServiceRequestDetailId = item.Id, Amount = amount });
                    item.PaymentStatus = PaymentStatusForItem.Paid;
                }

                var adjustmentPayment = new Payment
                {
                    ReceptionId = request.ReceptionId,
                    TotalAmount = totalAmountDifference,
                    Method = request.Method,
                    Note = request.Note,
                    PaymentType = PaymentType.Adjustment,
                    Status = PaymentStatus.Completed,
                    OriginalPaymentId = request.OriginalPaymentId,
                    PaymentDetails = adjustmentDetails
                };

                _context.Payments.Add(adjustmentPayment);

                originalPayment.Status = PaymentStatus.Adjusted;

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully created Adjustment Payment with Id: {AdjustmentPaymentId}", adjustmentPayment.Id);
                return new AdjustPaymentResult(adjustmentPayment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during AdjustPaymentCommand for OriginalPaymentId: {OriginalPaymentId}", request.OriginalPaymentId);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
