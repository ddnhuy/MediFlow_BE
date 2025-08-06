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
    public record UpdateExaminationServiceCommand(
        int ServiceId,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId,
        ExaminationService ExaminationService,
        List<ServiceTestParameterDto> ServiceTestParameters
    ) : ICommand<UpdateExaminationServiceResult>;

    public record UpdateExaminationServiceResult(int ServiceId);

    public class UpdateExaminationServiceCommandHandler : ICommandHandler<UpdateExaminationServiceCommand, UpdateExaminationServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceTestParameterRepository _serviceTestParameterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateExaminationServiceCommandHandler> _logger;

        public UpdateExaminationServiceCommandHandler(
            IServiceRepository serviceRepository,
            IServiceTestParameterRepository serviceTestParameterRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateExaminationServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _serviceTestParameterRepository = serviceTestParameterRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UpdateExaminationServiceResult> Handle(UpdateExaminationServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Get existing service with parameters
                var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
                if (service == null)
                {
                    throw new NotFoundException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                // Check if service is an examination service
                if (service.ServiceType != ServiceType.Test)
                {
                    throw new BadRequestException(ExceptionKey.SERVICE_IS_NOT_EXAMINATION_SERVICE);
                }

                // Update service properties
                service.ServiceCode = request.ServiceCode;
                service.ServiceName = request.ServiceName;
                service.UnitPrice = request.UnitPrice;
                service.DepartmentId = request.DepartmentId;
                service.ServiceType = ServiceType.Test;
                service.ExaminationService = request.ExaminationService;

                // Update service
                await _serviceRepository.UpdateAsync(service, cancellationToken);

                // Handle service test parameters
                if (request.ServiceTestParameters != null && request.ServiceTestParameters.Any())
                {
                    // Create new parameters
                    var newParameters = request.ServiceTestParameters.Select(p => new ServiceTestParameter
                    {
                        ServiceId = service.Id,
                        ParameterName = p.ParameterName,
                        Unit = p.Unit,
                        StandardValue = p.StandardValue,
                        EquipmentName = p.EquipmentName,
                        SpecimenType = p.SpecimenType
                    }).ToList();

                    // Update parameters (replace all existing ones)
                    await _serviceTestParameterRepository.UpdateRangeAsync(newParameters, cancellationToken);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Examination service with ID {ServiceId} has been updated", request.ServiceId);

                return new UpdateExaminationServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while updating examination service with ID {ServiceId}", request.ServiceId);
                throw;
            }
        }
    }
}