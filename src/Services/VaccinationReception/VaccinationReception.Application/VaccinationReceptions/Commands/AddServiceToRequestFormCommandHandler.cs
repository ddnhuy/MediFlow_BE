using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.IServiceClients;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddServiceToRequestFormCommandHandler : ICommandHandler<AddServiceToRequestFormCommand, AddServiceToRequestFormResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddServiceToRequestFormCommand> _logger;
        private readonly IHospitalServiceClient _hospitalServiceClient;

        public AddServiceToRequestFormCommandHandler(
            IApplicationDbContext context,
            IHospitalServiceClient hospitalServiceClient,
            ILogger<AddServiceToRequestFormCommand> logger)
        {
            _context = context;
            _hospitalServiceClient = hospitalServiceClient;
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

                var requestForm = await _context.RequestForms
                    .FirstOrDefaultAsync(rf => rf.ReceptionId == request.ReceptionId, cancellationToken);

                if (requestForm == null)
                {
                    requestForm = new RequestForm
                    {
                        ReceptionId = request.ReceptionId,
                        RequestNumber = UniqueStringGenerator.GenerateUniqueString()
                    };
                    await _context.RequestForms.AddAsync(requestForm, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (request.Services != null && request.Services.Any())
                {
                    var serviceIds = request.Services
                        .Select(s => s.ServiceId)
                        .ToList();
                    // Change mess broke
                    var services = await _hospitalServiceClient.GetServicesByIdsAsync(serviceIds, cancellationToken);

                    var serviceDict = services.ToDictionary(s => s.Id);

                    foreach (var service in request.Services)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.RequestFormId == requestForm.Id &&
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
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestFormId = requestForm.Id,
                                    ServiceId = service.ServiceId,
                                    Quantity = service.Quantity,
                                    UnitPrice = unitPrice
                                };
                                await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                            }
                        }
                        else
                        {
                            var serviceRequestDetail = new ServiceRequestDetail
                            {
                                RequestFormId = requestForm.Id,
                                ServiceId = service.ServiceId,
                                Quantity = service.Quantity,
                                UnitPrice = unitPrice
                            };
                            await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(request.GroupType) && request.GroupId.HasValue)
                {
                    // Change mess bro
                    var services = await _hospitalServiceClient.GetServicesByGroupAsync(
                        request.GroupId.Value,
                        request.GroupType,
                        cancellationToken);

                    foreach (var service in services)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.RequestFormId == requestForm.Id &&
                                srd.ServiceId == service.Id,
                                cancellationToken);

                        if (existingService != null)
                        {
                            if (existingService.PaymentStatus == PaymentStatusForItem.NotPaid)
                            {
                                existingService.Quantity += request.DefaultQuantity;
                                existingService.UnitPrice = service.UnitPrice;
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestFormId = requestForm.Id,
                                    ServiceId = service.Id,
                                    Quantity = request.DefaultQuantity,
                                    UnitPrice = service.UnitPrice
                                };
                                await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                            }
                        }
                        else
                        {
                            var serviceRequestDetail = new ServiceRequestDetail
                            {
                                RequestFormId = requestForm.Id,
                                ServiceId = service.Id,
                                Quantity = request.DefaultQuantity,
                                UnitPrice = service.UnitPrice
                            };
                            await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                        }
                    }
                }
                else
                {
                    throw new BadRequestException(ExceptionKey.INVALID_DATA);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return new AddServiceToRequestFormResult(requestForm.Id, requestForm.RequestNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling AddServiceToRequestFormCommand");
                throw;
            }
        }
    }
}