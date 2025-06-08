using BuildingBlocks.CQRS;
using HospitalService.Domain.Models;
using HospitalService.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record CreateServiceGroupCommand(
        string GroupName,
        List<int>? ServiceIds
    ) : ICommand<CreateServiceGroupResult>;
    public record CreateServiceGroupResult(int ServiceGroupId);
    public class CreateServiceGroupCommandHandler : ICommandHandler<CreateServiceGroupCommand, CreateServiceGroupResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateServiceGroupCommand> _logger;

        public CreateServiceGroupCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateServiceGroupCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateServiceGroupResult> Handle(CreateServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var serviceGroup = new ServiceGroup
                    {
                        GroupName = request.GroupName,
                    };

                    _context.ServiceGroups.Add(serviceGroup);
                    await _context.SaveChangesAsync(cancellationToken);

                    if (request.ServiceIds != null && request.ServiceIds.Any())
                    {
                        var serviceGroupServices = request.ServiceIds.Select(serviceId => new ServiceGroupService
                        {
                            ServiceGroupId = serviceGroup.Id,
                            ServiceId = serviceId,
                        });

                        _context.ServiceGroupServices.AddRange(serviceGroupServices);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    _logger.LogInformation("Created new service group with ID {ServiceGroupId}", serviceGroup.Id);
                    return new CreateServiceGroupResult(serviceGroup.Id);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating service group");
                throw;
            }
        }
    }
}