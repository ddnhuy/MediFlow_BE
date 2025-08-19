using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PayOSServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record CreatePayOSPaymentCommand(
            int PatientId,
            int ReceptionId,
            PaymentMethod Method,
            string? Note,
            List<int> ReceptionVaccinationIds,
            List<int> ServiceRequestDetailIds) : ICommand<CreatePayOSPaymentResult>;

    public record CreatePayOSPaymentResult(int PaymentId, string InvoiceNumber, string CheckoutUrl, string QrCode);

    public class CreatePayOSPaymentCommandHandler : ICommandHandler<CreatePayOSPaymentCommand, CreatePayOSPaymentResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreatePayOSPaymentCommandHandler> _logger;
        private readonly IPayOSService _payOSService;

        public CreatePayOSPaymentCommandHandler(
            IApplicationDbContext context,
            ILogger<CreatePayOSPaymentCommandHandler> logger,
            IPayOSService payOSService)
        {
            _context = context;
            _logger = logger;
            _payOSService = payOSService;
        }

        public async Task<CreatePayOSPaymentResult> Handle(CreatePayOSPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin CreatePayOSPaymentCommand for ReceptionId: {ReceptionId}", request.ReceptionId);

            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

                if (reception == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
                }

                reception.LastUpdatedAt = DateTime.UtcNow;

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
                    .Where(x => request.ServiceRequestDetailIds.Contains(x.Id)
                             && x.ReceptionId == request.ReceptionId
                             && x.PaymentStatus == PaymentStatusForItem.NotPaid)
                    .ToListAsync(cancellationToken);

                if (unpaidVaccinations.Count != (request.ReceptionVaccinationIds?.Count ?? 0) ||
                    unpaidServices.Count != (request.ServiceRequestDetailIds?.Count ?? 0))
                {
                    throw new BadRequestException(ExceptionKey.ONE_OR_MORE_ITEMS_ALREADY_PAID_OR_INVALID);
                }

                int totalAmount = 0;
                var paymentDetails = new List<PaymentDetail>();

                foreach (var vacc in unpaidVaccinations)
                {
                    var amount = vacc.UnitPrice * vacc.Quantity;
                    totalAmount += (int)amount;
                    paymentDetails.Add(new PaymentDetail { ReceptionVaccinationId = vacc.Id, Amount = amount });
                }

                foreach (var service in unpaidServices)
                {
                    var amount = service.UnitPrice * service.Quantity;
                    totalAmount += (int)amount;
                    paymentDetails.Add(new PaymentDetail { ServiceRequestDetailId = service.Id, Amount = amount });
                }

                var newPayment = new Payment
                {
                    ReceptionId = request.ReceptionId,
                    TotalAmount = totalAmount,
                    Method = request.Method,
                    Note = request.Note,
                    InvoiceNumber = UniqueStringGenerator.GenerateInvoiceNumber(),
                    PaymentType = PaymentType.Receipt,
                    Status = PaymentStatus.Pending,
                    PaymentDetails = paymentDetails
                };

                _context.Payments.Add(newPayment);
                await _context.SaveChangesAsync(cancellationToken);

                var data = await _payOSService.CreatePaymentLinkAsync(
                    UniqueIntGenerator.GenerateUniqueOrderId(),
                    totalAmount,
                    newPayment.InvoiceNumber,
                    cancellationToken);

                return new CreatePayOSPaymentResult(newPayment.Id, newPayment.InvoiceNumber, data.checkoutUrl, data.qrCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during CreatePayOSPaymentCommand for ReceptionId: {ReceptionId}", request.ReceptionId);
                throw;
            }
        }
    }
}
