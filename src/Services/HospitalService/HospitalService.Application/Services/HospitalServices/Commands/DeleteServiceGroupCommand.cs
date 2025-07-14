using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
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
    public record DeleteServiceGroupCommand(
        int Id
    ) : ICommand<DeleteServiceGroupResult>;
    public record DeleteServiceGroupResult(int ServiceGroupId);

    public class DeleteServiceGroupCommandHandler : ICommandHandler<DeleteServiceGroupCommand, DeleteServiceGroupResult>
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteServiceGroupCommand> _logger;

        public DeleteServiceGroupCommandHandler(
            IServiceGroupRepository serviceGroupRepository,
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteServiceGroupCommand> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DeleteServiceGroupResult> Handle(DeleteServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var serviceGroup = await _serviceGroupRepository.GetByIdAsync(request.Id);
                if (serviceGroup == null)
                    throw new NotFoundException(ExceptionKey.SERVICE_GROUP_NOT_FOUND);

                serviceGroup.IsCancelled = true;

                var serviceGroupServices = await _serviceGroupServiceRepository.GetByServiceGroupIdAsync(request.Id);
                await _serviceGroupServiceRepository.DeleteRangeAsync(serviceGroupServices);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Deleted service group {ServiceGroupId}", request.Id);
                return new DeleteServiceGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while deleting service group {ServiceGroupId}", request.Id);
                throw;
            }
        }
    }
}