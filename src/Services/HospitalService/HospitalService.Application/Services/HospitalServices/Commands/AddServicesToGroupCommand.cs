using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record AddServicesToGroupCommand(
        int ServiceGroupId,
        List<int> ServiceIds
    ) : ICommand<AddServicesToGroupResult>;
    public record AddServicesToGroupResult(int ServiceGroupId, int AddedServicesCount);
    public class AddServicesToGroupCommandHandler : ICommandHandler<AddServicesToGroupCommand, AddServicesToGroupResult>
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddServicesToGroupCommand> _logger;

        public AddServicesToGroupCommandHandler(
            IServiceGroupRepository serviceGroupRepository,
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<AddServicesToGroupCommand> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AddServicesToGroupResult> Handle(AddServicesToGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var serviceGroup = await _serviceGroupRepository.GetByIdAsync(request.ServiceGroupId);
                if (serviceGroup == null)
                    throw new NotFoundException(ExceptionKey.SERVICE_GROUP_NOT_FOUND);

                var existingServiceIds = await _serviceGroupServiceRepository.GetExistingServiceIdsAsync(request.ServiceGroupId);
                var newServiceIds = request.ServiceIds.Where(id => !existingServiceIds.Contains(id)).ToList();

                var serviceGroupServices = newServiceIds.Select(serviceId => new ServiceGroupService
                {
                    ServiceGroupId = request.ServiceGroupId,
                    ServiceId = serviceId
                });

                await _serviceGroupServiceRepository.AddRangeAsync(serviceGroupServices);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Added {Count} services to group {ServiceGroupId}", newServiceIds.Count, request.ServiceGroupId);
                return new AddServicesToGroupResult(request.ServiceGroupId, newServiceIds.Count);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while adding services to group {ServiceGroupId}", request.ServiceGroupId);
                throw;
            }
        }
    }
}
