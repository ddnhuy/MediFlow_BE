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
    public record UpdateServiceGroupCommand(
        int Id,
        string GroupName
    ) : ICommand<UpdateServiceGroupResult>;
    public record UpdateServiceGroupResult(int ServiceGroupId);
    public class UpdateServiceGroupCommandHandler : ICommandHandler<UpdateServiceGroupCommand, UpdateServiceGroupResult>
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateServiceGroupCommand> _logger;

        public UpdateServiceGroupCommandHandler(
            IServiceGroupRepository serviceGroupRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateServiceGroupCommand> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UpdateServiceGroupResult> Handle(UpdateServiceGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var serviceGroup = await _serviceGroupRepository.GetByIdAsync(request.Id);
                if (serviceGroup == null)
                    throw new NotFoundException(ExceptionKey.SERVICE_GROUP_NOT_FOUND);

                serviceGroup.GroupName = request.GroupName;

                await _serviceGroupRepository.UpdateAsync(serviceGroup);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Updated service group {ServiceGroupId}", request.Id);
                return new UpdateServiceGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while updating service group {ServiceGroupId}", request.Id);
                throw;
            }
        }
    }
}