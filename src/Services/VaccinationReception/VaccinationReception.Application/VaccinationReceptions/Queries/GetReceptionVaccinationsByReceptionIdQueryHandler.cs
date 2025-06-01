using BuildingBlocks.CQRS;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetReceptionVaccinationsByReceptionIdQueryHandler : IQueryHandler<GetReceptionVaccinationsByReceptionIdQuery, GetReceptionVaccinationsByReceptionIdResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetReceptionVaccinationsByReceptionIdQueryHandler> _logger;

        public GetReceptionVaccinationsByReceptionIdQueryHandler(
            ApplicationDbContext context,
            ILogger<GetReceptionVaccinationsByReceptionIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetReceptionVaccinationsByReceptionIdResult> Handle(GetReceptionVaccinationsByReceptionIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv => rv.ReceptionId == request.ReceptionId && !rv.IsCancelled)
                    .OrderBy(rv => rv.AppointmentDate)
                    .ToListAsync(cancellationToken);

                var receptionVaccinationDTOs = receptionVaccinations.Adapt<IEnumerable<ReceptionVaccinationDTO>>();

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