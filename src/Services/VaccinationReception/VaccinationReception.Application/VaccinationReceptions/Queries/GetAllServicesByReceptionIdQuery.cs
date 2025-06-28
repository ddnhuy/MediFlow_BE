using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetAllServicesByReceptionIdQuery(int ReceptionId) : IQuery<IEnumerable<ServiceRequestDetailDTO>>;

    public class GetAllServicesByReceptionIdQueryHandler
        : IQueryHandler<GetAllServicesByReceptionIdQuery, IEnumerable<ServiceRequestDetailDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ServiceRequestDetailDTO> _logger;
        private readonly IHospitalService _hospitalService;
        private readonly IInventoryService _inventoryService;

        public GetAllServicesByReceptionIdQueryHandler(
            IApplicationDbContext context,
            IHospitalService hospitalService,
            ILogger<ServiceRequestDetailDTO> logger,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _hospitalService = hospitalService;
            _inventoryService = inventoryService;
        }

        public async Task<IEnumerable<ServiceRequestDetailDTO>> Handle(
            GetAllServicesByReceptionIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var details = await _context.ServiceRequestDetails
                    .Include(d => d.RequestForm)
                    .Where(d => d.RequestForm.ReceptionId == request.ReceptionId)
                    .ToListAsync(cancellationToken);

                var serviceIds = details
                    .Select(d => d.ServiceId)
                    .Distinct()
                    .ToList();

                var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);
                var serviceDictionary = services.ToDictionary(s => s.Id, s => s);

                var result = details.Select(d =>
                {
                    if (!serviceDictionary.TryGetValue(d.ServiceId, out var serviceDto))
                    {
                        _logger.LogWarning("Service ID {ServiceId} not found in hospital service", d.ServiceId);
                        serviceDto = new()
                        {
                            ServiceCode = "N/A",
                            ServiceName = "Unknown Service"
                        };
                    }

                    return new ServiceRequestDetailDTO
                    {
                        Id = d.Id,
                        ServiceId = d.ServiceId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        InvoiceDate = d.InvoiceDate,
                        PaymentStatus = d.PaymentStatus,
                        RequestNumber = d.RequestForm?.RequestNumber ?? "N/A",
                        ServiceCode = serviceDto.ServiceCode ?? "",
                        ServiceName = serviceDto.ServiceName ?? ""
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAllServicesByReceptionIdQuery");
                throw;
            }
        }
    }
}
