using BuildingBlocks.CQRS;
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
    public record RemoveServicesFromGroupCommand(
        int ServiceGroupId,
        List<int> ServiceIds
    ) : ICommand<RemoveServicesFromGroupResult>;
    public record RemoveServicesFromGroupResult(int ServiceGroupId, int RemovedServicesCount);
    public class RemoveServicesFromGroupCommandHandler : ICommandHandler<RemoveServicesFromGroupCommand, RemoveServicesFromGroupResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveServicesFromGroupCommand> _logger;

        public RemoveServicesFromGroupCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveServicesFromGroupCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RemoveServicesFromGroupResult> Handle(RemoveServicesFromGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceGroupServices = await _context.ServiceGroupServices
                    .Where(sgs => sgs.ServiceGroupId == request.ServiceGroupId && request.ServiceIds.Contains(sgs.ServiceId))
                    .ToListAsync(cancellationToken);

                foreach (var serviceGroupService in serviceGroupServices)
                {
                    serviceGroupService.IsCancelled = true;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Removed {Count} services from group {ServiceGroupId}", serviceGroupServices.Count, request.ServiceGroupId);
                return new RemoveServicesFromGroupResult(request.ServiceGroupId, serviceGroupServices.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing services from group {ServiceGroupId}", request.ServiceGroupId);
                throw;
            }
        }
    }
}