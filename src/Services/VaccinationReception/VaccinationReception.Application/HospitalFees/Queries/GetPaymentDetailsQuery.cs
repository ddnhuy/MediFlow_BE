using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.HospitalFeeDTOs;

namespace VaccinationReception.Application.HospitalFees.Queries
{
    public record GetPaymentDetailsQuery(int PaymentId) : IQuery<PaymentWithDetailsDTO>;

    public class GetPaymentDetailsQueryHandler : IQueryHandler<GetPaymentDetailsQuery, PaymentWithDetailsDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetPaymentDetailsQueryHandler> _logger;
        private readonly IHospitalService _hospitalService;
        private readonly IInventoryService _inventoryService;

        public GetPaymentDetailsQueryHandler(
            IApplicationDbContext context,
            ILogger<GetPaymentDetailsQueryHandler> logger,
            IHospitalService hospitalService,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _hospitalService = hospitalService;
            _inventoryService = inventoryService;
        }


        public async Task<PaymentWithDetailsDTO> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting payment details for PaymentId: {PaymentId}", request.PaymentId);

            var payment = await _context.Payments
                .Include(p => p.PaymentDetails)
                    .ThenInclude(pd => pd.ServiceRequestDetail)
                .Include(p => p.PaymentDetails)
                    .ThenInclude(pd => pd.ReceptionVaccination)
                .FirstOrDefaultAsync(p => p.Id == request.PaymentId && !p.IsCancelled, cancellationToken);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found with Id: {PaymentId}", request.PaymentId);
                throw new NotFoundException(ExceptionKey.PAYMENT_NOT_FOUND);
            }
            var serviceIds = payment.PaymentDetails
                  .Where(pd => pd.ServiceRequestDetailId.HasValue && pd.ServiceRequestDetail != null)
                  .Select(pd => pd.ServiceRequestDetail!.ServiceId)
                  .Distinct()
                  .ToList();

            var vaccinationIds = payment.PaymentDetails
                   .Where(pd => pd.ReceptionVaccinationId.HasValue && pd.ReceptionVaccination != null)
                   .Select(pd => pd.ReceptionVaccination!.VaccineId)
                   .Distinct()
                   .ToList();

            var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);

            var vaccines = await _inventoryService.GetMedicineInformationAsync(vaccinationIds, cancellationToken);

            var serviceDict = services.ToDictionary(s => s.Id);

            var vaccineDict = vaccines.ToDictionary(s => s.MedicineId);

            var paymentDTO = new PaymentDTO(
                payment.Id,
                payment.ReceptionId,
                payment.TotalAmount,
                payment.Method,
                payment.Note,
                payment.ATMTransactionCode,
                payment.PaymentType,
                payment.InvoiceNumber,
                payment.OfficialInvoiceNumber,
                payment.Status,
                payment.OriginalPaymentId,
                payment.CreatedAt,
                payment.LastUpdatedAt
            );

            var paymentDetailsDTO = payment.PaymentDetails
                        .Where(pd => !pd.IsCancelled)
                        .Select(pd =>
                        {
                            string? serviceCode = null;
                            string? serviceName = null;
                            if (pd.ServiceRequestDetailId.HasValue && pd.ServiceRequestDetail != null)
                            {
                                var serviceId = pd.ServiceRequestDetail.ServiceId;
                                if (serviceDict.TryGetValue(serviceId, out var service))
                                {
                                    serviceCode = service.ServiceCode;
                                    serviceName = service.ServiceName;
                                }
                            }

                            else if (pd.ReceptionVaccinationId.HasValue && pd.ReceptionVaccination != null)
                            {
                                var vaccineId = pd.ReceptionVaccination.VaccineId;
                                if (vaccineDict.TryGetValue(vaccineId, out var vaccine))
                                {
                                    serviceCode = vaccine.VaccineTypeName;
                                    serviceName = vaccine.MedicineName;
                                }
                            }
                            return new PaymentDetailDTO(
                                pd.Id,
                                pd.PaymentId,
                                pd.ReceptionVaccinationId,
                                pd.ServiceRequestDetailId,
                                pd.Amount,
                                pd.IsReversed,
                                pd.CreatedAt,
                                pd.LastUpdatedAt,
                                serviceCode,
                                serviceName
                            );
                        })
                        .ToList();

            _logger.LogInformation("Found payment with {DetailCount} details for PaymentId: {PaymentId}",
                paymentDetailsDTO.Count, request.PaymentId);

            return new PaymentWithDetailsDTO(paymentDTO, paymentDetailsDTO);
        }
    }
}
