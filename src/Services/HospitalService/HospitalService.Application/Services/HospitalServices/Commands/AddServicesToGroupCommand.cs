using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using HospitalService.Domain.Models;
using HospitalService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record AddServicesToGroupCommand(
        int ServiceGroupId,
        List<int> ServiceIds
    ) : ICommand<AddServicesToGroupResult>;
    public record AddServicesToGroupResult(int ServiceGroupId, int AddedServicesCount);
    public class AddServicesToGroupCommandHandler : ICommandHandler<AddServicesToGroupCommand, AddServicesToGroupResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddServicesToGroupCommand> _logger;

        public AddServicesToGroupCommandHandler(
            ApplicationDbContext context,
            ILogger<AddServicesToGroupCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AddServicesToGroupResult> Handle(AddServicesToGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceGroup = await _context.ServiceGroups.FindAsync(new object[] { request.ServiceGroupId }, cancellationToken);
                if (serviceGroup == null)
                    throw new NotFoundException($"ServiceGroup with ID {request.ServiceGroupId} not found");

                var existingServices = await _context.ServiceGroupServices
                    .Where(sgs => sgs.ServiceGroupId == request.ServiceGroupId)
                    .Select(sgs => sgs.ServiceId)
                    .ToListAsync(cancellationToken);

                var newServices = request.ServiceIds
                    .Where(serviceId => !existingServices.Contains(serviceId))
                    .Select(serviceId => new ServiceGroupService
                    {
                        ServiceGroupId = request.ServiceGroupId,
                        ServiceId = serviceId,
                    })
                    .ToList();

                _context.ServiceGroupServices.AddRange(newServices);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Added {Count} services to group {ServiceGroupId}", newServices.Count, request.ServiceGroupId);
                return new AddServicesToGroupResult(request.ServiceGroupId, newServices.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding services to group {ServiceGroupId}", request.ServiceGroupId);
                throw;
            }
        }
    }
}
