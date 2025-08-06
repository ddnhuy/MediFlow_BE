using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.Enums;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record ServiceTestParameterDto(
        string ParameterName,
        string Unit,
        string StandardValue,
        string? EquipmentName,
        string? SpecimenType
    );

    public record CreateExaminationServiceCommand(
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId,
        ExaminationService ExaminationService,
        List<ServiceTestParameterDto> ServiceTestParameters
    ) : ICommand<CreateExaminationServiceResult>;

    public record CreateExaminationServiceResult(int ServiceId);

    public class CreateExaminationServiceCommandHandler : ICommandHandler<CreateExaminationServiceCommand, CreateExaminationServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceTestParameterRepository _serviceTestParameterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateExaminationServiceCommandHandler> _logger;

        public CreateExaminationServiceCommandHandler(
            IServiceRepository serviceRepository,
            IServiceTestParameterRepository serviceTestParameterRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateExaminationServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _serviceTestParameterRepository = serviceTestParameterRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateExaminationServiceResult> Handle(CreateExaminationServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var existingService = await _serviceRepository.GetByServiceCodesAsync(new List<string> { request.ServiceCode }, cancellationToken);
                if (existingService.Any())
                {
                    _logger.LogWarning("Service with code {ServiceCode} already exists", request.ServiceCode);
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.DUPLICATE_SERVICE_CODE);
                }

                var service = new Service
                {
                    ServiceCode = request.ServiceCode,
                    ServiceName = request.ServiceName,
                    UnitPrice = request.UnitPrice,
                    DepartmentId = request.DepartmentId,
                    ServiceType = ServiceType.Test,
                    ExaminationService = request.ExaminationService
                };

                await _serviceRepository.AddAsync(service, cancellationToken);
                // Save to get the generated ServiceId
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                // Now add parameters with the correct ServiceId
                var parameters = request.ServiceTestParameters.Select(p => new ServiceTestParameter
                {
                    ServiceId = service.Id,
                    ParameterName = p.ParameterName,
                    Unit = p.Unit,
                    StandardValue = p.StandardValue,
                    EquipmentName = p.EquipmentName,
                    SpecimenType = p.SpecimenType
                }).ToList();

                await _serviceTestParameterRepository.AddRangeAsync(parameters, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Created new examination service with ID {ServiceId}", service.Id);
                return new CreateExaminationServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while creating examination service");
                throw;
            }
        }
    }
}