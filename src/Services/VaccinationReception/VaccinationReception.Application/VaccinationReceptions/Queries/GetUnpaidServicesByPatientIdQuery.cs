using BuildingBlocks.CQRS;
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
    public record GetUnpaidServicesByPatientIdQuery(int PatientId) : IQuery<UnpaidServicesResponseDTO>;
    public class GetUnpaidServicesByPatientIdQueryHandler : IQueryHandler<GetUnpaidServicesByPatientIdQuery, UnpaidServicesResponseDTO>
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

        public async Task<UnpaidServicesResponseDTO> Handle(GetUnpaidServicesByPatientIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var receptionIds = await _context.Receptions
                    .Where(r => r.PatientId == request.PatientId && !r.IsCancelled)
                    .Select(r => r.Id)
                    .ToListAsync(cancellationToken);

                if (!receptionIds.Any())
                {
                    return new UnpaidServicesResponseDTO(new List<UnpaidServiceDTO>(), new List<UnpaidVaccinationDTO>());
                }

                var unpaidServices = await _context.ServiceRequestDetails
                    .Include(srd => srd.RequestForm)
                    .Where(srd =>
                        receptionIds.Contains(srd.RequestForm.ReceptionId) &&
                        srd.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !srd.IsCancelled)
                    .ToListAsync(cancellationToken);

                var serviceIds = unpaidServices.Select(srd => srd.ServiceId).Distinct().ToList();

                var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);
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
                        receptionIds.Contains(rv.ReceptionId) &&
                        rv.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !rv.IsCancelled)
                    .Select(rv => new UnpaidVaccinationDTO(
                        rv.Id,
                        rv.RequestNumber,
                        rv.VaccineId,
                        string.Empty,
                        string.Empty,
                        rv.Quantity,
                        rv.UnitPrice,
                        rv.CreatedAt
                    ))
                    .ToListAsync(cancellationToken);

                var vaccineIds = unpaidVaccinations.Select(uv => uv.VaccineId).Distinct().ToList();

                if (vaccineIds.Any())
                {
                    var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);

                    var medicineLookup = medicineInformationList
                        .Where(m => m.IsSuccess)
                        .ToDictionary(m => m.MedicineId, m => m);

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
                _logger.LogError(ex, "Error occurred while handling GetUnpaidServicesByPatientIdQuery for PatientId: {PatientId}", request.PatientId);
                throw;
            }
        }
    }
}