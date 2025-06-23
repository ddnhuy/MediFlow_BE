using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.IServiceClients;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetUnpaidServicesQueryHandler : IQueryHandler<GetUnpaidServicesQuery, UnpaidServicesResponseDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUnpaidServicesQuery> _logger;
        private readonly IHospitalServiceClient _hospitalServiceClient;
        private readonly IInventoryService _inventoryService;

        public GetUnpaidServicesQueryHandler(
            IApplicationDbContext context,
            IHospitalServiceClient hospitalServiceClient,
            ILogger<GetUnpaidServicesQuery> logger,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _hospitalServiceClient = hospitalServiceClient;
            _inventoryService = inventoryService;
        }

        public async Task<UnpaidServicesResponseDTO> Handle(GetUnpaidServicesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var unpaidServices = await _context.ServiceRequestDetails
                    .Include(srd => srd.RequestForm)
                    .Where(srd =>
                        srd.RequestForm.ReceptionId == request.ReceptionId &&
                        srd.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !srd.IsCancelled)
                    .ToListAsync(cancellationToken);

                var serviceIds = unpaidServices.Select(srd => srd.ServiceId).Distinct().ToList();

                var services = await _hospitalServiceClient.GetServicesByIdsAsync(serviceIds, cancellationToken);

                var serviceDictionary = services.ToDictionary(s => s.Id, s => s);

                var unpaidServicesDTO = unpaidServices.Select(srd =>
                {
                    var service = serviceDictionary.GetValueOrDefault(srd.ServiceId);
                    return new UnpaidServiceDTO(
                        srd.Id,
                        srd.RequestForm.RequestNumber,
                        srd.ServiceId,
                        service?.ServiceName ?? "Unknown Service", 
                        srd.Quantity,
                        srd.UnitPrice,
                        srd.CreatedAt
                    );
                }).ToList();

                var unpaidVaccinations = await _context.ReceptionVaccinations
                    .Where(rv =>
                        rv.ReceptionId == request.ReceptionId &&
                        rv.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !rv.IsCancelled)
                    .Select(rv => new UnpaidVaccinationDTO(
                        rv.Id,
                        rv.RequestNumber,
                        rv.VaccineId,
                        string.Empty, // Will be populated with VaccineTypeName
                        string.Empty, // Will be populated with MedicineName
                        rv.Quantity,
                        rv.UnitPrice,
                        rv.CreatedAt
                    ))
                    .ToListAsync(cancellationToken);

                // Get unique vaccine IDs to fetch medicine information
                var vaccineIds = unpaidVaccinations.Select(uv => uv.VaccineId).Distinct().ToList();

                if (vaccineIds.Any())
                {
                    var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);

                    // Create lookup dictionary for medicine information
                    var medicineLookup = medicineInformationList
                        .Where(m => m.IsSuccess)
                        .ToDictionary(m => m.MedicineId, m => m);

                    // Update unpaid vaccinations with medicine information
                    var updatedUnpaidVaccinations = unpaidVaccinations.Select(uv =>
                    {
                        if (medicineLookup.TryGetValue(uv.VaccineId, out var medicineInfo))
                        {
                            return new UnpaidVaccinationDTO(
                                uv.Id,
                                uv.RequestNumber,
                                uv.VaccineId,
                                medicineInfo.VaccineTypeName ?? string.Empty,
                                medicineInfo.MedicineName ?? string.Empty,
                                uv.Quantity,
                                uv.UnitPrice,
                                uv.CreatedAt
                            );
                        }
                        else
                        {
                            return new UnpaidVaccinationDTO(
                                uv.Id,
                                uv.RequestNumber,
                                uv.VaccineId,
                                string.Empty,
                                string.Empty,
                                uv.Quantity,
                                uv.UnitPrice,
                                uv.CreatedAt
                            );
                        }
                    }).ToList();

                    return new UnpaidServicesResponseDTO(unpaidServicesDTO, updatedUnpaidVaccinations);
                }

                return new UnpaidServicesResponseDTO(unpaidServicesDTO, unpaidVaccinations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetUnpaidServicesQuery");
                throw;
            }
        }
    }
}