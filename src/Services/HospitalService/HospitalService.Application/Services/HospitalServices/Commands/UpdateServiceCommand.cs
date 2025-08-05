using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Enums;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record UpdateServiceCommand(
        int ServiceId,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId,
        ServiceType ServiceType

    ) : ICommand<UpdateServiceResult>;

    public record UpdateServiceResult(int ServiceId);

    // Command Handler
    public class UpdateServiceCommandHandler : ICommandHandler<UpdateServiceCommand, UpdateServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateServiceCommandHandler> _logger;

        public UpdateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UpdateServiceResult> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
                if (service == null)
                {
                    throw new NotFoundException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                service.ServiceCode = request.ServiceCode;
                service.ServiceName = request.ServiceName;
                service.UnitPrice = request.UnitPrice;
                service.DepartmentId = request.DepartmentId;
                service.ServiceType = request.ServiceType;

                await _serviceRepository.UpdateAsync(service, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Service with ID {ServiceId} has been updated", request.ServiceId);

                return new UpdateServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while updating service with ID {ServiceId}", request.ServiceId);
                throw;
            }
        }
    }
}
