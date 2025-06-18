using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record DeleteServiceCommand(int ServiceId) : ICommand<DeleteServiceResult>;

    public record DeleteServiceResult(int ServiceId);

    public class DeleteServiceCommandHandler : ICommandHandler<DeleteServiceCommand, DeleteServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteServiceCommandHandler> _logger;

        public DeleteServiceCommandHandler(
            IServiceRepository serviceRepository,
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DeleteServiceResult> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
                if (service == null)
                {
                    throw new NotFoundException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                // Delete related ServiceGroupServices
                var serviceGroupServices = await _serviceGroupServiceRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                if (serviceGroupServices.Any())
                {
                    foreach (var sgs in serviceGroupServices)
                    {
                        sgs.IsCancelled = true;
                    }
                }
                await _serviceGroupServiceRepository.UpdateRangeAsync(serviceGroupServices);

                // Delete related DiseaseGroupServices
                var diseaseGroupServices = await _diseaseGroupServiceRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                if (diseaseGroupServices.Any())
                {
                    foreach (var dgs in diseaseGroupServices)
                    {
                        dgs.IsCancelled = true;
                    }
                }
                await _diseaseGroupServiceRepository.UpdateRangeAsync(diseaseGroupServices);

                // Soft delete the service
                service.IsCancelled = true;
                await _serviceRepository.UpdateAsync(service, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Service with ID {ServiceId} has been soft deleted", request.ServiceId);

                return new DeleteServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while deleting service with ID {ServiceId}", request.ServiceId);
                throw;
            }
        }
    }
}
