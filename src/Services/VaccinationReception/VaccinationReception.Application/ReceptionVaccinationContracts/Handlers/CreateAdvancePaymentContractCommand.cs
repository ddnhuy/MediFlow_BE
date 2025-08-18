using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PayOSServices;
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
    public record CreateAdvancePaymentContractResult(
        PaymentContract PaymentContract,
        string? CheckoutUrl = null,
        string? QrCode = null);

    public class CreateAdvancePaymentContractCommandHandler : ICommandHandler<CreateAdvancePaymentContractCommand, CreateAdvancePaymentContractResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateAdvancePaymentContractCommandHandler> _logger;
        private readonly IPayOSService _payOSService;

        public CreateAdvancePaymentContractCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateAdvancePaymentContractCommandHandler> logger,
            IPayOSService payOSService)
        {
            _context = context;
            _logger = logger;
            _payOSService = payOSService;
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

                var paymentStatus = request.PaymentMethod == PaymentMethod.BankTransfer
                    ? PaymentStatus.Pending
                    : PaymentStatus.Completed;

                var paymentContract = new PaymentContract
                {
                    ContractId = request.ContractId,
                    InvoiceNumber = await UniqueStringGenerator.GenerateInvoiceNumberAsync(),
                    VATInvoiceNumber = request.VATInvoiceNumber,
                    InvoiceType = InvoiceType.AdvancePayment,
                    TotalAmount = request.AdvanceAmount,
                    PaymentMethod = request.PaymentMethod,
                    Status = paymentStatus,
                    TaxCode = request.TaxCode,
                    OrganizationName = request.OrganizationName,
                };

                if (paymentStatus == PaymentStatus.Completed)
                {
                    contract.AdvanceAmount = request.AdvanceAmount;
                }

                await _context.PaymentContracts.AddAsync(paymentContract, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new PaymentContract with Id: {Id} for ContractId: {ContractId}",
                    paymentContract.Id, request.ContractId);

                string? checkoutUrl = null;
                string? qrCode = null;

                if (request.PaymentMethod == PaymentMethod.BankTransfer)
                {
                    try
                    {
                        var payOSData = await _payOSService.CreatePaymentLinkAsync(
                            UniqueIntGenerator.GenerateUniqueOrderId(),
                            (int)request.AdvanceAmount,
                            paymentContract.InvoiceNumber,
                            cancellationToken);

                        checkoutUrl = payOSData.checkoutUrl;
                        qrCode = payOSData.qrCode;

                        _logger.LogInformation("Created PayOS payment link for PaymentContract Id: {Id}", paymentContract.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create PayOS payment link for PaymentContract Id: {Id}", paymentContract.Id);
                    }
                }

                return new CreateAdvancePaymentContractResult(paymentContract, checkoutUrl, qrCode);
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