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
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PayOSServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record CreateFinalInvoicePaymentContractCommand(
        int ContractId,
        PaymentMethod PaymentMethod,
        string? VATInvoiceNumber,
        string TaxCode,
        string OrganizationName
    ) : ICommand<CreateFinalInvoicePaymentContractResultDTO>;

    public class CreateFinalInvoicePaymentContractCommandHandler : ICommandHandler<CreateFinalInvoicePaymentContractCommand, CreateFinalInvoicePaymentContractResultDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateFinalInvoicePaymentContractCommandHandler> _logger;
        private readonly IHospitalService _hospitalService;
        private readonly IInventoryService _inventoryService;
        private readonly IPayOSService _payOSService;


        public CreateFinalInvoicePaymentContractCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateFinalInvoicePaymentContractCommandHandler> logger,
            IHospitalService hospitalService,
            IInventoryService inventoryService,
            IPayOSService payOSService)
        {
            _context = context;
            _logger = logger;
            _hospitalService = hospitalService;
            _inventoryService = inventoryService;
            _payOSService = payOSService;
        }

        public async Task<CreateFinalInvoicePaymentContractResultDTO> Handle(CreateFinalInvoicePaymentContractCommand request, CancellationToken cancellationToken)
        {
            var contract = await _context.Contracts
                .Include(c => c.ServiceDetails)
                .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

            if (contract == null)
            {
                _logger.LogWarning("Contract with ID {ContractId} not found.", request.ContractId);
                throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);
            }

            var receptionIds = await _context.Receptions
                .Where(r => r.ContractId == request.ContractId)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            if (!receptionIds.Any())
            {
                _logger.LogWarning("No receptions found for contract with ID {ContractId}.", request.ContractId);
                throw new NotFoundException(ExceptionKey.NOT_FOUND_RECEPTION);
            }

            var receptionVaccinations = await _context.ReceptionVaccinations
                .Where(rv => receptionIds.Contains(rv.ReceptionId))
                .ToListAsync(cancellationToken);

            var serviceRequestDetails = await _context.ServiceRequestDetails
                .Where(srd => receptionIds.Contains(srd.ReceptionId))
                .ToListAsync(cancellationToken);

            var vaccineGroups = receptionVaccinations
                .GroupBy(rv => rv.VaccineId)
                .ToDictionary(g => g.Key, g => new
                {
                    Quantity = g.Sum(x => x.Quantity),
                    Amount = g.Sum(x => x.Quantity * x.UnitPrice)
                });

            var serviceGroups = serviceRequestDetails
                .GroupBy(srd => srd.ServiceId)
                .ToDictionary(g => g.Key, g => new
                {
                    Quantity = g.Sum(x => x.Quantity),
                    Amount = g.Sum(x => x.Quantity * x.UnitPrice)
                });

            decimal totalActualAmount = 0;
            foreach (var detail in contract.ServiceDetails)
            {
                if (detail.VaccineId.HasValue && vaccineGroups.TryGetValue(detail.VaccineId.Value, out var vaccine))
                {
                    detail.ActualQuantity = vaccine.Quantity;
                    detail.ActualTotalAmount = (int?)vaccine.Amount;
                    totalActualAmount += vaccine.Amount;
                }
                else if (detail.ServiceId.HasValue && serviceGroups.TryGetValue(detail.ServiceId.Value, out var service))
                {
                    detail.ActualQuantity = service.Quantity;
                    detail.ActualTotalAmount = (int?)service.Amount;
                    totalActualAmount += service.Amount;
                }
            }

            var advanceAmount = contract.AdvanceAmount ?? 0;
            var finalAmount = totalActualAmount - advanceAmount;

            contract.ActualAmount = totalActualAmount;

            var paymentContract = new PaymentContract
            {
                ContractId = contract.Id,
                InvoiceNumber = UniqueStringGenerator.GenerateInvoiceNumber(),
                VATInvoiceNumber = request.VATInvoiceNumber,
                InvoiceType = InvoiceType.FinalInvoice,
                TotalAmount = finalAmount,
                PaymentMethod = request.PaymentMethod,
                Status = PaymentStatus.Pending,
                TaxCode = request.TaxCode,
                OrganizationName = request.OrganizationName
            };

            await _context.PaymentContracts.AddAsync(paymentContract, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            string? checkoutUrl = null;
            string? qrCode = null;

            if (request.PaymentMethod == PaymentMethod.BankTransfer)
            {
                try
                {
                    var payOSData = await _payOSService.CreatePaymentLinkAsync(
                        UniqueIntGenerator.GenerateUniqueOrderId(),
                        (int)finalAmount,
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

            var serviceIds = contract.ServiceDetails.Where(x => x.ServiceId.HasValue).Select(x => x.ServiceId!.Value).Distinct().ToList();
            var vaccineIds = contract.ServiceDetails.Where(x => x.VaccineId.HasValue).Select(x => x.VaccineId!.Value).Distinct().ToList();

            var serviceDict = new Dictionary<int, string>();
            var vaccineDict = new Dictionary<int, string>();

            if (serviceIds.Any())
            {
                var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);
                serviceDict = services.ToDictionary(x => x.Id, x => x.ServiceName);
            }
            if (vaccineIds.Any())
            {
                var medicines = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);
                vaccineDict = medicines.ToDictionary(x => x.MedicineId, x => x.MedicineName);
            }

            var contractServiceDetailsDto = contract.ServiceDetails.Select(detail =>
            {
                string? name = null;
                if (detail.ServiceId.HasValue)
                {
                    serviceDict.TryGetValue(detail.ServiceId.Value, out name);
                }
                else if (detail.VaccineId.HasValue)
                {
                    vaccineDict.TryGetValue(detail.VaccineId.Value, out name);
                }

                return new ContractServiceDetailDTO
                {
                    Id = detail.Id,
                    ServiceId = detail.ServiceId,
                    VaccineId = detail.VaccineId,
                    UnitPrice = detail.UnitPrice,
                    Quantity = detail.Quantity,
                    TotalAmount = detail.TotalAmount,
                    ActualQuantity = detail.ActualQuantity,
                    ActualTotalAmount = detail.ActualTotalAmount,
                    Name = name
                };
            }).ToList();

            var paymentContractDto = new PaymentContractDTO
            {
                Id = paymentContract.Id,
                InvoiceNumber = paymentContract.InvoiceNumber,
                VATInvoiceNumber = paymentContract.VATInvoiceNumber,
                InvoiceType = paymentContract.InvoiceType,
                TotalAmount = paymentContract.TotalAmount,
                PaymentMethod = paymentContract.PaymentMethod,
                Status = paymentContract.Status,
                TaxCode = paymentContract.TaxCode,
                OrganizationName = paymentContract.OrganizationName,
            };

            return new CreateFinalInvoicePaymentContractResultDTO
            {
                ContractId = contract.Id,
                PaymentContract = paymentContractDto,
                Details = contractServiceDetailsDto,
                CheckoutUrl = checkoutUrl,
                QrCode = qrCode
            };
        }
    }
}