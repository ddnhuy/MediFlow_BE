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
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddServiceToRequestFormCommandHandler : ICommandHandler<AddServiceToRequestFormCommand, AddServiceToRequestFormResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddServiceToRequestFormCommand> _logger;

        public AddServiceToRequestFormCommandHandler(
            ApplicationDbContext context,
            ILogger<AddServiceToRequestFormCommand> logger)
        {
            _context = context;
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
                    foreach (var service in request.Services)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.RequestFormId == requestForm.Id &&
                                srd.ServiceId == service.ServiceId,
                                cancellationToken);

                        if (existingService != null)
                        {
                            if (!existingService.IsPaid)
                            {
                                existingService.Quantity += service.Quantity;
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestFormId = requestForm.Id,
                                    ServiceId = service.ServiceId,
                                    Quantity = service.Quantity
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
                                Quantity = service.Quantity
                            };
                            await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(request.GroupType) && request.GroupId.HasValue)
                {
                    List<int> serviceIds;
                    if (request.GroupType.ToLower() == "servicegroup")
                    {
                        serviceIds = await _context.ServiceGroupServices
                            .Where(sgs => sgs.ServiceGroupId == request.GroupId)
                            .Select(sgs => sgs.ServiceId)
                            .ToListAsync(cancellationToken);
                    }
                    else if (request.GroupType.ToLower() == "diseasegroup")
                    {
                        serviceIds = await _context.DiseaseGroupServices
                            .Where(dgs => dgs.DiseaseGroupId == request.GroupId)
                            .Select(dgs => dgs.ServiceId)
                            .ToListAsync(cancellationToken);
                    }
                    else
                    {
                        throw new BadRequestException(ExceptionKey.INVALID_GROUP_TYPE);
                    }

                    foreach (var serviceId in serviceIds)
                    {
                        var existingService = await _context.ServiceRequestDetails
                            .FirstOrDefaultAsync(srd =>
                                srd.RequestFormId == requestForm.Id &&
                                srd.ServiceId == serviceId,
                                cancellationToken);

                        if (existingService != null)
                        {
                            if (!existingService.IsPaid)
                            {
                                existingService.Quantity += request.DefaultQuantity;
                            }
                            else
                            {
                                var serviceRequestDetail = new ServiceRequestDetail
                                {
                                    RequestFormId = requestForm.Id,
                                    ServiceId = serviceId,
                                    Quantity = request.DefaultQuantity
                                };
                                await _context.ServiceRequestDetails.AddAsync(serviceRequestDetail, cancellationToken);
                            }
                        }
                        else
                        {
                            var serviceRequestDetail = new ServiceRequestDetail
                            {
                                RequestFormId = requestForm.Id,
                                ServiceId = serviceId,
                                Quantity = request.DefaultQuantity
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