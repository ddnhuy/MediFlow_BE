using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using HospitalService.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record UpdateServiceGroupCommand(
        int Id,
        string GroupName
    ) : ICommand<UpdateServiceGroupResult>;
    public record UpdateServiceGroupResult(int ServiceGroupId);
    public class UpdateServiceGroupCommandHandler : ICommandHandler<UpdateServiceGroupCommand, UpdateServiceGroupResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateServiceGroupCommand> _logger;

        public UpdateServiceGroupCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateServiceGroupCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateServiceGroupResult> Handle(UpdateServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var serviceGroup = await _context.ServiceGroups.FindAsync(new object[] { request.Id }, cancellationToken);
                if (serviceGroup == null)
                    throw new NotFoundException($"ServiceGroup with ID {request.Id} not found");

                serviceGroup.GroupName = request.GroupName;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated service group {ServiceGroupId}", request.Id);
                return new UpdateServiceGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service group {ServiceGroupId}", request.Id);
                throw;
            }
        }
    }
}