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
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record UpdatePaymentContractStatusCommand(
            int ContractId,
            int PaymentContractId,
            PaymentStatus PaymentStatus
        ) : ICommand<bool>;

    public class UpdatePaymentContractStatusCommandHandler : ICommandHandler<UpdatePaymentContractStatusCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdatePaymentContractStatusCommandHandler> _logger;

        public UpdatePaymentContractStatusCommandHandler(
            IApplicationDbContext context,
            ILogger<UpdatePaymentContractStatusCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePaymentContractStatusCommand request, CancellationToken cancellationToken)
        {
            var paymentContract = await _context.PaymentContracts
                .FirstOrDefaultAsync(x => x.Id == request.PaymentContractId && x.ContractId == request.ContractId, cancellationToken);

            if (paymentContract == null)
                throw new NotFoundException(ExceptionKey.PAYMENT_NOT_FOUND);

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(x => x.Id == request.ContractId, cancellationToken);

            if (contract == null)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);

            PaymentStatusForItem newPaymentStatusForItem;
            if (request.PaymentStatus == PaymentStatus.Completed)
            {
                paymentContract.Status = PaymentStatus.Completed;
                contract.Status = ContractStatus.Completed;
                newPaymentStatusForItem = PaymentStatusForItem.Paid; 
            }
            else if (request.PaymentStatus == PaymentStatus.Cancelled)
            {
                paymentContract.Status = PaymentStatus.Cancelled;
                contract.Status = ContractStatus.Cancelled;
                newPaymentStatusForItem = PaymentStatusForItem.Cancelled;
            }
            else
            {
                paymentContract.Status = request.PaymentStatus;
               
                newPaymentStatusForItem = PaymentStatusForItem.NotPaid; 
            }

            var receptions = await _context.Receptions
                .Where(r => r.ContractId == request.ContractId)
                .ToListAsync(cancellationToken);

            if (receptions.Any())
            {
                var receptionIds = receptions.Select(r => r.Id).ToList();

                var serviceRequestDetails = await _context.ServiceRequestDetails
                    .Where(srd => receptionIds.Contains(srd.ReceptionId))
                    .ToListAsync(cancellationToken);

                foreach (var detail in serviceRequestDetails)
                {
                    detail.PaymentStatus = newPaymentStatusForItem;
                }

                // Update ReceptionVaccinations
                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv => receptionIds.Contains(rv.ReceptionId))
                    .ToListAsync(cancellationToken);

                foreach (var vaccination in receptionVaccinations)
                {
                    vaccination.PaymentStatus = newPaymentStatusForItem;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
