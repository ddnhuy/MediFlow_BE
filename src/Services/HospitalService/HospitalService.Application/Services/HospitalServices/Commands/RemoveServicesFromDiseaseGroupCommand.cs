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
    public record RemoveServicesFromDiseaseGroupCommand(
            int DiseaseGroupId,
            List<int> ServiceIds
        ) : ICommand<RemoveServicesFromDiseaseGroupResult>;
    public record RemoveServicesFromDiseaseGroupResult(int DiseaseGroupId, int RemovedServicesCount);
    public class RemoveServicesFromDiseaseGroupCommandHandler : ICommandHandler<RemoveServicesFromDiseaseGroupCommand, RemoveServicesFromDiseaseGroupResult>
    {
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveServicesFromDiseaseGroupCommand> _logger;

        public RemoveServicesFromDiseaseGroupCommandHandler(
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<RemoveServicesFromDiseaseGroupCommand> logger)
        {
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RemoveServicesFromDiseaseGroupResult> Handle(RemoveServicesFromDiseaseGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var diseaseGroupServices = await _diseaseGroupServiceRepository.GetByServiceGroupIdAsync(request.DiseaseGroupId);
                var servicesToRemove = diseaseGroupServices
                    .Where(dgs => request.ServiceIds.Contains(dgs.ServiceId))
                    .ToList();

                await _diseaseGroupServiceRepository.DeleteRangeAsync(servicesToRemove);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Removed {Count} services from disease group {DiseaseGroupId}", servicesToRemove.Count, request.DiseaseGroupId);
                return new RemoveServicesFromDiseaseGroupResult(request.DiseaseGroupId, servicesToRemove.Count);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while removing services from disease group {DiseaseGroupId}", request.DiseaseGroupId);
                throw;
            }
        }
    }
}
