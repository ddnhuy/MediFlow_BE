using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.IServiceClients;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetUnpaidServicesQueryHandler : IQueryHandler<GetUnpaidServicesQuery, UnpaidServicesResponseDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetUnpaidServicesQuery> _logger;
        private readonly IHospitalServiceClient _hospitalServiceClient;

        public GetUnpaidServicesQueryHandler(
            ApplicationDbContext context,
            IHospitalServiceClient hospitalServiceClient,
            ILogger<GetUnpaidServicesQuery> logger)
        {
            _context = context;
            _logger = logger;
            _hospitalServiceClient = hospitalServiceClient;
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
                        rv.Quantity,
                        rv.UnitPrice,
                        rv.CreatedAt
                    ))
                    .ToListAsync(cancellationToken);

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