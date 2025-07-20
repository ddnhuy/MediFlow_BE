using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddServiceToRequestFormCommandHandler : ICommandHandler<AddServiceToRequestFormCommand, AddServiceToRequestFormResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddServiceToRequestFormCommand> _logger;
        private readonly IHospitalService _hospitalService;

        public AddServiceToRequestFormCommandHandler(
            IApplicationDbContext context,
            IHospitalService hospitalService,
            ILogger<AddServiceToRequestFormCommand> logger)
        {
            _context = context;
            _hospitalService = hospitalService;
            _logger = logger;
        }

        public async Task<AddServiceToRequestFormResult> Handle(AddServiceToRequestFormCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

                if (reception == null)
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);

                var processedServiceReferences = new List<ServiceIdAndRequestNumberDTO>();

                if (request.Services != null && request.Services.Any())
                {
                    var serviceIds = request.Services
                        .Select(s => s.ServiceId)
                        .ToList();
                    // Change mess broke
                    var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);

                    var serviceDict = services.ToDictionary(s => s.Id);

                    foreach (var service in request.Services)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.ReceptionId == reception.Id &&
                                srd.ServiceId == service.ServiceId,
                                cancellationToken);

                        var unitPrice = serviceDict.TryGetValue(service.ServiceId, out var matchedService)
                                      ? matchedService.UnitPrice
                                      : 0;

                        if (existingService != null)
                        {
                            if (existingService.PaymentStatus == PaymentStatusForItem.NotPaid)
                            {
                                existingService.Quantity += service.Quantity;
                                existingService.UnitPrice = unitPrice;

                                processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                    existingService.ServiceId,
                                    existingService.RequestNumber
                                ));
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                                    ReceptionId = reception.Id,
                                    ServiceId = service.ServiceId,
                                    Quantity = service.Quantity,
                                    UnitPrice = unitPrice
                                };
                                await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);

                                processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                    serviceRequestDetail.ServiceId,
                                    serviceRequestDetail.RequestNumber
                                ));
                            }
                        }
                        else
                        {
                            var serviceRequestDetail = new ServiceRequestDetail
                            {
                                RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                                ReceptionId = reception.Id,
                                ServiceId = service.ServiceId,
                                Quantity = service.Quantity,
                                UnitPrice = unitPrice
                            };
                            await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);

                            processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                serviceRequestDetail.ServiceId,
                                serviceRequestDetail.RequestNumber
                            ));
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(request.GroupType) && request.GroupId.HasValue)
                {
                    var services = await _hospitalService.GetServicesByGroupAsync(
                        request.GroupId.Value,
                        request.GroupType,
                        cancellationToken);

                    foreach (var service in services)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.ReceptionId == reception.Id &&
                                srd.ServiceId == service.Id,
                                cancellationToken);

                        if (existingService != null)
                        {
                            if (existingService.PaymentStatus == PaymentStatusForItem.NotPaid)
                            {
                                existingService.Quantity += request.DefaultQuantity;
                                existingService.UnitPrice = service.UnitPrice;

                                processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                    existingService.ServiceId,
                                    existingService.RequestNumber
                                ));
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                                    ReceptionId = reception.Id,
                                    ServiceId = service.Id,
                                    Quantity = request.DefaultQuantity,
                                    UnitPrice = service.UnitPrice
                                };
                                await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);

                                processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                    serviceRequestDetail.ServiceId,
                                    serviceRequestDetail.RequestNumber
                                ));
                            }
                        }
                        else
                        {
                            var serviceRequestDetail = new ServiceRequestDetail
                            {
                                RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                                ReceptionId = reception.Id,
                                ServiceId = service.Id,
                                Quantity = request.DefaultQuantity,
                                UnitPrice = service.UnitPrice
                            };
                            await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);

                            processedServiceReferences.Add(new ServiceIdAndRequestNumberDTO(
                                serviceRequestDetail.ServiceId,
                                serviceRequestDetail.RequestNumber
                            ));
                        }
                    }
                }
                else
                {
                    throw new BadRequestException(ExceptionKey.INVALID_DATA);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return new AddServiceToRequestFormResult(reception.Id, processedServiceReferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling AddServiceToRequestFormCommand");
                throw;
            }
        }
    }
}