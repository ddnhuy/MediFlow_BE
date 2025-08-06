using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Enums;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record DeleteExaminationServiceCommand(int ServiceId) : ICommand<DeleteExaminationServiceResult>;

    public record DeleteExaminationServiceResult(int ServiceId);

    public class DeleteExaminationServiceCommandHandler : ICommandHandler<DeleteExaminationServiceCommand, DeleteExaminationServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceGroupServiceRepository _serviceGroupServiceRepository;
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IServiceTestParameterRepository _serviceTestParameterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteExaminationServiceCommandHandler> _logger;

        public DeleteExaminationServiceCommandHandler(
            IServiceRepository serviceRepository,
            IServiceGroupServiceRepository serviceGroupServiceRepository,
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IServiceTestParameterRepository serviceTestParameterRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteExaminationServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _serviceGroupServiceRepository = serviceGroupServiceRepository;
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _serviceTestParameterRepository = serviceTestParameterRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DeleteExaminationServiceResult> Handle(DeleteExaminationServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
                if (service == null)
                {
                    throw new NotFoundException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                // Validate that it's an examination service
                if (service.ServiceType != ServiceType.Test)
                {
                    _logger.LogWarning("Service with ID {ServiceId} is not an examination service", request.ServiceId);
                    throw new BadRequestException(ExceptionKey.SERVICE_IS_NOT_EXAMINATION_SERVICE);
                }

                // Delete related ServiceTestParameters
                var serviceTestParameters = await _serviceTestParameterRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                if (serviceTestParameters.Any())
                {
                    await _serviceTestParameterRepository.DeleteRangeAsync(serviceTestParameters, cancellationToken);
                    _logger.LogInformation("Deleted {Count} service test parameters for service {ServiceId}",
                        serviceTestParameters.Count(), request.ServiceId);
                }

                // Delete related ServiceGroupServices
                var serviceGroupServices = await _serviceGroupServiceRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                if (serviceGroupServices.Any())
                {
                    foreach (var sgs in serviceGroupServices)
                    {
                        sgs.IsCancelled = true;
                    }
                    await _serviceGroupServiceRepository.UpdateRangeAsync(serviceGroupServices);
                    _logger.LogInformation("Deleted {Count} service group services for service {ServiceId}",
                        serviceGroupServices.Count(), request.ServiceId);
                }

                // Delete related DiseaseGroupServices
                var diseaseGroupServices = await _diseaseGroupServiceRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                if (diseaseGroupServices.Any())
                {
                    foreach (var dgs in diseaseGroupServices)
                    {
                        dgs.IsCancelled = true;
                    }
                    await _diseaseGroupServiceRepository.UpdateRangeAsync(diseaseGroupServices);
                    _logger.LogInformation("Deleted {Count} disease group services for service {ServiceId}",
                        diseaseGroupServices.Count(), request.ServiceId);
                }

                // Soft delete the examination service
                service.IsCancelled = true;
                await _serviceRepository.UpdateAsync(service, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Examination service with ID {ServiceId} has been soft deleted", request.ServiceId);

                return new DeleteExaminationServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while deleting examination service with ID {ServiceId}", request.ServiceId);
                throw;
            }
        }
    }
}