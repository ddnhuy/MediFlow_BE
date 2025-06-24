using BuildingBlocks.CQRS;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetReceptionVaccinationsByReceptionIdQueryHandler : IQueryHandler<GetReceptionVaccinationsByReceptionIdQuery, GetReceptionVaccinationsByReceptionIdResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetReceptionVaccinationsByReceptionIdQueryHandler> _logger;
        private readonly IInventoryService _inventoryService;

        public GetReceptionVaccinationsByReceptionIdQueryHandler(
            IApplicationDbContext context,
            ILogger<GetReceptionVaccinationsByReceptionIdQueryHandler> logger, IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task<GetReceptionVaccinationsByReceptionIdResult> Handle(GetReceptionVaccinationsByReceptionIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv => rv.ReceptionId == request.ReceptionId && !rv.IsCancelled)
                    .OrderBy(rv => rv.AppointmentDate)
                    .ToListAsync(cancellationToken);

                var vaccineIds = receptionVaccinations.Select(rv => rv.VaccineId).Distinct().ToList();

                var medicineInfos = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);

                var receptionVaccinationDTOs = receptionVaccinations.Select(rv =>
                {
                    var dto = rv.Adapt<ReceptionVaccinationDTO>();
                    var medicine = medicineInfos.FirstOrDefault(m => m.MedicineId == rv.VaccineId);
                    if (medicine != null)
                    {
                        dto.VaccineName = medicine.MedicineName;
                        dto.VaccineTypeName = medicine.VaccineTypeName;
                    }
                    return dto;
                }).ToList();

                _logger.LogInformation("Retrieved {Count} reception vaccinations for ReceptionId {ReceptionId}",
                    receptionVaccinations.Count, request.ReceptionId);

                return new GetReceptionVaccinationsByReceptionIdResult(receptionVaccinationDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving reception vaccinations for ReceptionId {ReceptionId}",
                    request.ReceptionId);
                throw;
            }
        }
    }
}