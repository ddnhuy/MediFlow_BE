using BuildingBlocks.CQRS;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
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
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateServiceGroupCommand> _logger;

        public CreateServiceGroupCommandHandler(
            IServiceGroupRepository serviceGroupRepository,
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateServiceGroupCommand> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateServiceGroupResult> Handle(CreateServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var serviceGroup = new ServiceGroup
                {
                    GroupName = request.GroupName,
                };

                await _serviceGroupRepository.AddAsync(serviceGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.ServiceIds != null && request.ServiceIds.Any())
                {
                    var serviceGroupServices = request.ServiceIds.Select(serviceId => new ServiceGroupService
                    {
                        ServiceGroupId = serviceGroup.Id,
                        ServiceId = serviceId,
                    });

                    await _serviceGroupServiceRepository.AddRangeAsync(serviceGroupServices);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Created new service group with ID {ServiceGroupId}", serviceGroup.Id);
                return new CreateServiceGroupResult(serviceGroup.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while creating service group: {Message}", ex.InnerException?.Message ?? ex.Message);
                throw;
            }

        }
    }
}