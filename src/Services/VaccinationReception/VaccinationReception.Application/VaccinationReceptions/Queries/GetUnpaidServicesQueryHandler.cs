using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetUnpaidServicesQueryHandler : IQueryHandler<GetUnpaidServicesQuery, UnpaidServicesResponseDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUnpaidServicesQuery> _logger;

        public GetUnpaidServicesQueryHandler(
            IApplicationDbContext context,
            ILogger<GetUnpaidServicesQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UnpaidServicesResponseDTO> Handle(GetUnpaidServicesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var unpaidServices = await _context.ServiceRequestDetails
                    .Include(srd => srd.Service)
                    .Include(srd => srd.RequestForm)
                    .Where(srd =>
                        srd.RequestForm.ReceptionId == request.ReceptionId &&
                        !srd.IsPaid &&
                        !srd.IsCancelled)
                    .Select(srd => new UnpaidServiceDTO(
                        srd.Id,
                        srd.RequestForm.RequestNumber,
                        srd.ServiceId,
                        srd.Service.ServiceName,
                        srd.Quantity,
                        srd.UnitPrice,
                        srd.CreatedAt
                    ))
                    .ToListAsync(cancellationToken);

                var unpaidVaccinations = await _context.ReceptionVaccinations
                    .Where(rv =>
                        rv.ReceptionId == request.ReceptionId &&
                        !rv.IsPaid &&
                        !rv.IsCancelled)
                    .Select(rv => new UnpaidVaccinationDTO(
                        rv.Id,
                        rv.VaccineId,
                        rv.Quantity,
                        rv.CreatedAt
                    ))
                    .ToListAsync(cancellationToken);

                return new UnpaidServicesResponseDTO(unpaidServices, unpaidVaccinations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetUnpaidServicesQuery");
                throw;
            }
        }
    }
}