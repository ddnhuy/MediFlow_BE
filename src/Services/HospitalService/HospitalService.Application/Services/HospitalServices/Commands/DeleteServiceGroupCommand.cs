using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
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
    public record DeleteServiceGroupCommand(
        int Id
    ) : ICommand<DeleteServiceGroupResult>;
    public record DeleteServiceGroupResult(int ServiceGroupId);

    public class DeleteServiceGroupCommandHandler : ICommandHandler<DeleteServiceGroupCommand, DeleteServiceGroupResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteServiceGroupCommand> _logger;

        public DeleteServiceGroupCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteServiceGroupCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteServiceGroupResult> Handle(DeleteServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceGroup = await _context.ServiceGroups.FindAsync(new object[] { request.Id }, cancellationToken);
                if (serviceGroup == null)
                    throw new NotFoundException($"ServiceGroup with ID {request.Id} not found");

                serviceGroup.IsCancelled = true;

                var serviceGroupServices = await _context.ServiceGroupServices
                                        .Where(sgs => sgs.ServiceGroupId == request.Id)
                                        .ToListAsync(cancellationToken);

                foreach (var serviceGroupService in serviceGroupServices)
                {
                    serviceGroupService.IsCancelled = true;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted service group {ServiceGroupId}", request.Id);
                return new DeleteServiceGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting service group {ServiceGroupId}", request.Id);
                throw;
            }
        }
    }
}