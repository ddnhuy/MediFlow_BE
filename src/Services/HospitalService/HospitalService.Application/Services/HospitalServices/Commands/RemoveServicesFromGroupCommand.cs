using BuildingBlocks.CQRS;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Repositories;
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
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveServicesFromGroupCommand> _logger;

        public RemoveServicesFromGroupCommandHandler(
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<RemoveServicesFromGroupCommand> logger)
        {
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RemoveServicesFromGroupResult> Handle(RemoveServicesFromGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var serviceGroupServices = await _serviceGroupServiceRepository.GetByServiceGroupIdAsync(request.ServiceGroupId);
                var servicesToRemove = serviceGroupServices
                    .Where(sgs => request.ServiceIds.Contains(sgs.ServiceId))
                    .ToList();

                await _serviceGroupServiceRepository.DeleteRangeAsync(servicesToRemove);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Removed {Count} services from group {ServiceGroupId}", servicesToRemove.Count, request.ServiceGroupId);
                return new RemoveServicesFromGroupResult(request.ServiceGroupId, servicesToRemove.Count);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while removing services from group {ServiceGroupId}", request.ServiceGroupId);
                throw;
            }
        }
    }
}