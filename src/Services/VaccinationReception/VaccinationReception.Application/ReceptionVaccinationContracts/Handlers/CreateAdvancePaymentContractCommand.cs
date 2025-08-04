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

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record CreateAdvancePaymentContractCommand(
            int ContractId,
            decimal AdvanceAmount,
            PaymentMethod PaymentMethod,
            string? VATInvoiceNumber,
            string TaxCode,
            string OrganizationName) : ICommand<CreateAdvancePaymentContractResult>;
    public record CreateAdvancePaymentContractResult(PaymentContract PaymentContract);

    public class CreateAdvancePaymentContractCommandHandler : ICommandHandler<CreateAdvancePaymentContractCommand, CreateAdvancePaymentContractResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateAdvancePaymentContractCommandHandler> _logger;

        public CreateAdvancePaymentContractCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateAdvancePaymentContractCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateAdvancePaymentContractResult> Handle(CreateAdvancePaymentContractCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin CreateAdvancePaymentContractCommand for ContractId: {ContractId}", request.ContractId);

            try
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

                if (contract == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);
                }

                if (contract.Status != ContractStatus.Active)
                {
                    throw new BadRequestException(ExceptionKey.CONTRACT_IS_NOT_ACTIVE);
                }

                if (request.AdvanceAmount > contract.ContractValue)
                {
                    throw new BadRequestException(ExceptionKey.ADVANCE_AMOUNT_EXCEEDS_CONTRACT_VALUE);
                }

                var paymentContract = new PaymentContract
                {
                    ContractId = request.ContractId,
                    InvoiceNumber = await UniqueStringGenerator.GenerateInvoiceNumberAsync(),
                    VATInvoiceNumber = request.VATInvoiceNumber,
                    InvoiceType = InvoiceType.AdvancePayment,
                    TotalAmount = request.AdvanceAmount,
                    PaymentMethod = request.PaymentMethod,
                    Status = PaymentStatus.Completed,
                    TaxCode = request.TaxCode,
                    OrganizationName = request.OrganizationName,
                };
                contract.AdvanceAmount = request.AdvanceAmount;

                await _context.PaymentContracts.AddAsync(paymentContract, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new PaymentContract with Id: {Id} for ContractId: {ContractId}",
                    paymentContract.Id, request.ContractId);

                return new CreateAdvancePaymentContractResult(paymentContract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating advance payment contract for ContractId: {ContractId}",
                    request.ContractId);
                throw;
            }
        }
    }
}