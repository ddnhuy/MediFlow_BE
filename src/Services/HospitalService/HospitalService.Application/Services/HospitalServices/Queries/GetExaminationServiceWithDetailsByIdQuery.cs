using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Enums;
using HospitalService.Application.DTOs;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Queries
{
    public record ExaminationServiceDetailDTO(
        int Id,
        string ServiceCode,
        string ServiceName,
        ServiceType? ServiceType,
        decimal UnitPrice,
        int DepartmentId,
        ExaminationService? ExaminationService,
        ICollection<ServiceTestParameter>? ServiceTestParameters,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        int CreatedBy,
        int LastUpdatedBy
    );

    public record GetExaminationServiceWithDetailsByIdQuery(int ServiceId) : IQuery<ExaminationServiceDetailDTO>;

    public class GetExaminationServiceWithDetailsByIdQueryHandler : IQueryHandler<GetExaminationServiceWithDetailsByIdQuery, ExaminationServiceDetailDTO>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetExaminationServiceWithDetailsByIdQueryHandler> _logger;

        public GetExaminationServiceWithDetailsByIdQueryHandler(
            IServiceRepository serviceRepository,
            ILogger<GetExaminationServiceWithDetailsByIdQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<ExaminationServiceDetailDTO> Handle(GetExaminationServiceWithDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting examination service details by ID: {ServiceId}", request.ServiceId);

            try
            {
                if (request.ServiceId <= 0)
                {
                    _logger.LogWarning("Invalid service ID provided: {ServiceId}", request.ServiceId);
                    throw new BadRequestException(ExceptionKey.INVALID_SERVICE_ID);
                }

                var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);

                if (service == null)
                {
                    _logger.LogWarning("Examination service not found with ID: {ServiceId}", request.ServiceId);
                    throw new BadRequestException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                // Validate that it's an examination service
                if (service.ServiceType != ServiceType.Test)
                {
                    _logger.LogWarning("Service with ID {ServiceId} is not an examination service", request.ServiceId);
                    throw new BadRequestException(ExceptionKey.SERVICE_IS_NOT_EXAMINATION_SERVICE);
                }

                var serviceDetailDTO = new ExaminationServiceDetailDTO(
                    service.Id,
                    service.ServiceCode,
                    service.ServiceName,
                    service.ServiceType,
                    service.UnitPrice,
                    service.DepartmentId,
                    service.ExaminationService,
                    service.ServiceTestParameters,
                    service.CreatedAt,
                    service.LastUpdatedAt,
                    service.CreatedBy,
                    service.LastUpdatedBy
                );

                _logger.LogInformation("Successfully retrieved examination service details with ID {ServiceId}", request.ServiceId);

                return serviceDetailDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting examination service details by ID {ServiceId}", request.ServiceId);
                throw;
            }
        }
    }
}