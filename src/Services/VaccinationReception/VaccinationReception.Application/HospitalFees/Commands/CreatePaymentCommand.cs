using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.HospitalFees.Commands
{
    public record CreatePaymentCommand(
        int PatientId,
        int ReceptionId,
        PaymentMethod Method,
        string? Note,
        List<int> ReceptionVaccinationIds,
        List<int> ServiceRequestDetailIds) : ICommand<CreatePaymentResult>;

    public record CreatePaymentResult(int PaymentId);

    public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, CreatePaymentResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;

        public CreatePaymentCommandHandler(IApplicationDbContext context, ILogger<CreatePaymentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreatePaymentResult> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin CreatePaymentCommand for ReceptionId: {ReceptionId}", request.ReceptionId);

            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

                if (reception == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
                }

                if (reception.PatientId != request.PatientId)
                {
                    throw new BadRequestException(ExceptionKey.RECEPTION_DOES_NOT_BELONG_TO_PATIENT);
                }

                var unpaidVaccinations = await _context.ReceptionVaccinations
                    .Where(x => request.ReceptionVaccinationIds.Contains(x.Id)
                             && x.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync(cancellationToken);

                var unpaidServices = await _context.ServiceRequestDetails
                    .Include(x => x.RequestForm)
                    .Where(x => request.ServiceRequestDetailIds.Contains(x.Id)
                             && x.RequestForm.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync(cancellationToken);

                if (unpaidVaccinations.Count != (request.ReceptionVaccinationIds?.Count ?? 0) ||
                    unpaidServices.Count != (request.ServiceRequestDetailIds?.Count ?? 0))
                {
                    _logger.LogWarning("Some items are already paid or do not exist.");
                    throw new BadRequestException(ExceptionKey.ONE_OR_MORE_ITEMS_ALREADY_PAID_OR_INVALID);
                }

                decimal totalAmount = 0;
                var paymentDetails = new List<PaymentDetail>();

                foreach (var vacc in unpaidVaccinations)
                {
                    var amount = vacc.UnitPrice * vacc.Quantity;
                    totalAmount += amount;
                    paymentDetails.Add(new PaymentDetail { ReceptionVaccinationId = vacc.Id, Amount = amount });
                    vacc.PaymentStatus = PaymentStatusForItem.Paid;
                    vacc.InvoiceDate = DateTime.UtcNow;
                }

                foreach (var service in unpaidServices)
                {
                    var amount = service.UnitPrice * service.Quantity;
                    totalAmount += amount;
                    paymentDetails.Add(new PaymentDetail { ServiceRequestDetailId = service.Id, Amount = amount });
                    service.PaymentStatus = PaymentStatusForItem.Paid;
                    service.InvoiceDate = DateTime.UtcNow;
                }

                var newPayment = new Payment
                {
                    ReceptionId = request.ReceptionId,
                    TotalAmount = totalAmount,
                    Method = request.Method,
                    Note = request.Note,
                    InvoiceNumber = UniqueStringGenerator.GenerateInvoiceNumber(),
                    PaymentType = PaymentType.Receipt,
                    Status = PaymentStatus.Completed,
                    PaymentDetails = paymentDetails
                };

                _context.Payments.Add(newPayment);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created Payment with Id: {PaymentId}", newPayment.Id);
                return new CreatePaymentResult(newPayment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during CreatePaymentCommand for ReceptionId: {ReceptionId}", request.ReceptionId);
                throw;
            }
        }
    }
}