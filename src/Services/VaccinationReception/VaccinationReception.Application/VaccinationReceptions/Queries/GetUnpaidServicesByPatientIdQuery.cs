using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
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
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetUnpaidServicesByPatientIdQuery(int PatientId) : IQuery<UnpaidServicesByPatientResponseDTO>;
    public class GetUnpaidServicesByPatientIdQueryHandler : IQueryHandler<GetUnpaidServicesByPatientIdQuery, UnpaidServicesByPatientResponseDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUnpaidServicesByPatientIdQuery> _logger;
        private readonly IHospitalService _hospitalService;
        private readonly IInventoryService _inventoryService;

        public GetUnpaidServicesByPatientIdQueryHandler(
            IApplicationDbContext context,
            IHospitalService hospitalService,
            ILogger<GetUnpaidServicesByPatientIdQuery> logger,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _hospitalService = hospitalService;
            _inventoryService = inventoryService;
        }

        public async Task<UnpaidServicesByPatientResponseDTO> Handle(GetUnpaidServicesByPatientIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var latestReceptionId = await _context.Receptions
                    .Where(r => r.PatientId == request.PatientId && !r.IsCancelled)
                    .OrderByDescending(r => r.ReceptionDate)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (latestReceptionId == 0)
                {
                    return new UnpaidServicesByPatientResponseDTO(
                        ReceptionId: 0,
                        Vaccinations: new List<UnpaidVaccinationDTO>(),
                        Services: new List<UnpaidServiceDTO>()
                    );
                }
                var unpaidServices = await _context.ServiceRequestDetails
                    .Include(srd => srd.RequestForm)
                    .Where(srd =>
                        srd.RequestForm.ReceptionId == latestReceptionId &&
                        srd.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !srd.IsCancelled)
                    .ToListAsync(cancellationToken);

                var serviceIds = unpaidServices.Select(srd => srd.ServiceId).Distinct().ToList();
                var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);
                var serviceDictionary = services.ToDictionary(s => s.Id, s => s);

                var unpaidServicesDTO = unpaidServices.Select(srd =>
                {
                    var serviceName = serviceDictionary.TryGetValue(srd.ServiceId, out var svc)
                        ? svc.ServiceName
                        : "Unknown Service";

                    return new UnpaidServiceDTO(
                        srd.Id,
                        srd.RequestForm.RequestNumber,
                        srd.ServiceId,
                        serviceName ?? string.Empty,
                        srd.Quantity,
                        srd.UnitPrice,
                        srd.CreatedAt
                    );
                }).ToList();

                var unpaidVaccinationsRaw = await _context.ReceptionVaccinations
                    .Where(rv =>
                        rv.ReceptionId == latestReceptionId &&
                        rv.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !rv.IsCancelled)
                    .ToListAsync(cancellationToken);

                var vaccineIds = unpaidVaccinationsRaw.Select(rv => rv.VaccineId).Distinct().ToList();

                Dictionary<int, GetMedicineInformationResponse> medicineLookup = new();

                if (vaccineIds.Any())
                {
                    var medicineInfoList = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);
                    medicineLookup = medicineInfoList
                        .Where(m => m.IsSuccess)
                        .ToDictionary(m => m.MedicineId, m => m);
                }

                var unpaidVaccinations = unpaidVaccinationsRaw.Select(rv =>
                {
                    medicineLookup.TryGetValue(rv.VaccineId, out var medicine);

                    return new UnpaidVaccinationDTO(
                        rv.Id,
                        rv.RequestNumber,
                        rv.VaccineId,
                        medicine?.VaccineTypeName ?? string.Empty,
                        medicine?.MedicineName ?? string.Empty,
                        rv.Quantity,
                        rv.UnitPrice,
                        rv.CreatedAt
                    );
                }).ToList();

                return new UnpaidServicesByPatientResponseDTO(
                         ReceptionId: latestReceptionId,
                         Vaccinations: unpaidVaccinations,
                         Services: unpaidServicesDTO
                     );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetUnpaidServicesByPatientIdQuery for PatientId: {PatientId}", request.PatientId);
                throw;
            }
        }
    }
}