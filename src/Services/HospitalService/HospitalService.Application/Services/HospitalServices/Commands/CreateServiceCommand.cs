using BuildingBlocks.CQRS;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record CreateServiceCommand(
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId
    ) : ICommand<CreateServiceResult>;
    public record CreateServiceResult(int ServiceId);

    public class CreateServiceCommandHandler : ICommandHandler<CreateServiceCommand, CreateServiceResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateServiceCommand> _logger;

        public CreateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateServiceCommand> logger)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateServiceResult> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var service = new Service
                {
                    ServiceCode = request.ServiceCode,
                    ServiceName = request.ServiceName,
                    UnitPrice = request.UnitPrice,
                    DepartmentId = request.DepartmentId,
                };

                await _serviceRepository.AddAsync(service);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Created new service with ID {ServiceId}", service.Id);
                return new CreateServiceResult(service.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while creating service");
                throw;
            }
        }
    }
}