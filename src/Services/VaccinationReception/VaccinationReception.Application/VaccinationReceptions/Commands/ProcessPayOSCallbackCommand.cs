using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.PayOSDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PayOSServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record ProcessPayOSCallbackCommand(JsonElement Payload) : ICommand<ProcessPayOSCallbackResult>;

    public record ProcessPayOSCallbackResult(bool Success, string Message);

    public class ProcessPayOSCallbackCommandHandler : ICommandHandler<ProcessPayOSCallbackCommand, ProcessPayOSCallbackResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProcessPayOSCallbackCommandHandler> _logger;
        private readonly IPayOSService _payOSService;

        public ProcessPayOSCallbackCommandHandler(
            IApplicationDbContext context,
            ILogger<ProcessPayOSCallbackCommandHandler> logger,
            IPayOSService payOSService)
        {
            _context = context;
            _logger = logger;
            _payOSService = payOSService;
        }

        public async Task<ProcessPayOSCallbackResult> Handle(ProcessPayOSCallbackCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin ProcessPayOSCallbackCommand");
            try
            {
                if (!request.Payload.TryGetProperty("data", out var dataElement) ||
                    !request.Payload.TryGetProperty("signature", out var signatureElement))
                {
                    _logger.LogInformation("Webhook verification request - missing data or signature");
                    return new ProcessPayOSCallbackResult(true, "Webhook verification successful");
                }

                var dataObject = dataElement;
                var signature = signatureElement.GetString();

                if (dataObject.ValueKind == JsonValueKind.Undefined || string.IsNullOrEmpty(signature))
                {
                    _logger.LogInformation("Webhook verification request - empty data or signature");
                    return new ProcessPayOSCallbackResult(true, "Webhook verification successful");
                }

                var dataString = JsonSerializer.Serialize(dataObject);

                var paymentData = JsonSerializer.Deserialize<PayOSCallbackData>(dataString);
                if (paymentData == null)
                {
                    _logger.LogError("Failed to deserialize PayOS callback data");
                    return new ProcessPayOSCallbackResult(false, "Invalid data format");
                }

                _logger.LogInformation("Processing PayOS callback for OrderCode: {OrderCode}, Code: {Code}, Description: {Description}",
                    paymentData.OrderCode, paymentData.Code, paymentData.Description);

                var paymentId = paymentData.OrderCode;
                var invoiceNumber = InvoiceNumberHelper.RestoreInvoiceNumber(paymentData.Description);

                var payment = await _context.Payments
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Reception)
                    .FirstOrDefaultAsync(
                        p => p.InvoiceNumber.ToLower() == invoiceNumber.ToLower(),
                        cancellationToken);

                if (payment != null)
                {
                    if (paymentData.Code == "00" && paymentData.Desc == "success")
                    {
                        await ProcessSuccessfulPayment(payment, paymentData, cancellationToken);
                        _logger.LogInformation("Successfully processed paid payment for OrderCode: {OrderCode}", paymentData.OrderCode);
                        return new ProcessPayOSCallbackResult(true, "Payment processed successfully");
                    }
                    else
                    {
                        await ProcessFailedPayment(payment, paymentData, cancellationToken);
                        _logger.LogInformation("Successfully processed failed payment for OrderCode: {OrderCode}, Code: {Code}",
                            paymentData.OrderCode, paymentData.Code);
                        return new ProcessPayOSCallbackResult(true, "Payment failed");
                    }
                }

                var paymentContract = await _context.PaymentContracts
                    .Include(pc => pc.Contract)
                    .FirstOrDefaultAsync(
                        pc => pc.InvoiceNumber.ToLower() == invoiceNumber.ToLower(),
                        cancellationToken);

                if (paymentContract != null)
                {
                    if (paymentData.Code == "00" && paymentData.Desc == "success")
                    {
                        await ProcessSuccessfulPaymentContract(paymentContract, paymentData, cancellationToken);
                        _logger.LogInformation("Successfully processed paid payment contract for OrderCode: {OrderCode}", paymentData.OrderCode);
                        return new ProcessPayOSCallbackResult(true, "Payment contract processed successfully");
                    }
                    else
                    {
                        await ProcessFailedPaymentContract(paymentContract, paymentData, cancellationToken);
                        _logger.LogInformation("Successfully processed failed payment contract for OrderCode: {OrderCode}, Code: {Code}",
                            paymentData.OrderCode, paymentData.Code);
                        return new ProcessPayOSCallbackResult(true, "Payment contract failed");
                    }
                }

                _logger.LogInformation("Payment not found for OrderCode: {OrderCode} - treating as webhook verification", paymentData.OrderCode);
                return new ProcessPayOSCallbackResult(true, "Webhook verification successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ProcessPayOSCallbackCommand");
                return new ProcessPayOSCallbackResult(false, "Internal error");
            }
        }

        private async Task ProcessSuccessfulPayment(Payment payment, PayOSCallbackData paymentData, CancellationToken cancellationToken)
        {
            payment.Status = PaymentStatus.Completed;
            payment.ATMTransactionCode = paymentData.Reference;

            var receptionVaccinationIds = payment.PaymentDetails
                .Where(pd => pd.ReceptionVaccinationId.HasValue)
                .Select(pd => pd.ReceptionVaccinationId.Value)
                .ToList();

            var serviceRequestDetailIds = payment.PaymentDetails
                .Where(pd => pd.ServiceRequestDetailId.HasValue)
                .Select(pd => pd.ServiceRequestDetailId.Value)
                .ToList();

            if (receptionVaccinationIds.Any())
            {
                var vaccinations = await _context.ReceptionVaccinations
                    .Where(rv => receptionVaccinationIds.Contains(rv.Id))
                    .ToListAsync(cancellationToken);

                foreach (var vaccination in vaccinations)
                {
                    vaccination.PaymentStatus = PaymentStatusForItem.Paid;
                    vaccination.InvoiceDate = DateTime.UtcNow;
                }
            }

            if (serviceRequestDetailIds.Any())
            {
                var services = await _context.ServiceRequestDetails
                    .Where(srd => serviceRequestDetailIds.Contains(srd.Id))
                    .ToListAsync(cancellationToken);

                foreach (var service in services)
                {
                    service.PaymentStatus = PaymentStatusForItem.Paid;
                    service.InvoiceDate = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessFailedPayment(Payment payment, PayOSCallbackData paymentData, CancellationToken cancellationToken)
        {
            payment.Status = PaymentStatus.Cancelled;
            payment.ATMTransactionCode = paymentData.Reference;
            payment.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessSuccessfulPaymentContract(PaymentContract paymentContract, PayOSCallbackData paymentData, CancellationToken cancellationToken)
        {
            paymentContract.Status = PaymentStatus.Completed;
            paymentContract.ATMCode = paymentData.Reference;

            var contract = paymentContract.Contract;
            if (contract != null)
            {
                if (paymentContract.InvoiceType == InvoiceType.AdvancePayment)
                {
                    contract.AdvanceAmount = paymentContract.TotalAmount;
                    _logger.LogInformation("Updated contract AdvanceAmount to {Amount} for ContractId: {ContractId}",
                        paymentContract.TotalAmount, contract.Id);
                }

                if (paymentContract.InvoiceType != InvoiceType.AdvancePayment)
                {
                    contract.Status = ContractStatus.Completed;
                }
            }

            if (paymentContract.InvoiceType != InvoiceType.AdvancePayment)
            {
                var newPaymentStatusForItem = PaymentStatusForItem.Paid;
                await UpdateContractRelatedItems(paymentContract.ContractId, newPaymentStatusForItem, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessFailedPaymentContract(PaymentContract paymentContract, PayOSCallbackData paymentData, CancellationToken cancellationToken)
        {
            paymentContract.Status = PaymentStatus.Cancelled;
            paymentContract.ATMCode = paymentData.Reference;
            paymentContract.LastUpdatedAt = DateTime.UtcNow;

            var contract = paymentContract.Contract;
            if (contract != null)
            {
                if (paymentContract.InvoiceType != InvoiceType.AdvancePayment)
                {
                    contract.Status = ContractStatus.Cancelled;
                }
            }

            if (paymentContract.InvoiceType != InvoiceType.AdvancePayment)
            {
                var newPaymentStatusForItem = PaymentStatusForItem.Cancelled;
                await UpdateContractRelatedItems(paymentContract.ContractId, newPaymentStatusForItem, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateContractRelatedItems(int contractId, PaymentStatusForItem newPaymentStatusForItem, CancellationToken cancellationToken)
        {
            var receptions = await _context.Receptions
                .Where(r => r.ContractId == contractId)
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
                    if (newPaymentStatusForItem == PaymentStatusForItem.Paid)
                    {
                        detail.InvoiceDate = DateTime.UtcNow;
                    }
                }

                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv => receptionIds.Contains(rv.ReceptionId))
                    .ToListAsync(cancellationToken);

                foreach (var vaccination in receptionVaccinations)
                {
                    vaccination.PaymentStatus = newPaymentStatusForItem;
                    if (newPaymentStatusForItem == PaymentStatusForItem.Paid)
                    {
                        vaccination.InvoiceDate = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}