using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public record AddServicesToDiseaseGroupCommand(
            int DiseaseGroupId,
            List<int> ServiceIds
        ) : ICommand<AddServicesToDiseaseGroupResult>;
    public record AddServicesToDiseaseGroupResult(int DiseaseGroupId, int AddedServicesCount);
    public class AddServicesToDiseaseGroupCommandHandler : ICommandHandler<AddServicesToDiseaseGroupCommand, AddServicesToDiseaseGroupResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddServicesToDiseaseGroupCommand> _logger;

        public AddServicesToDiseaseGroupCommandHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<AddServicesToDiseaseGroupCommand> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AddServicesToDiseaseGroupResult> Handle(AddServicesToDiseaseGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var diseaseGroup = await _diseaseGroupRepository.GetByIdAsync(request.DiseaseGroupId);
                if (diseaseGroup == null)
                    throw new NotFoundException(ExceptionKey.DISEASE_GROUP_NOT_FOUND);

                var existingServiceIds = await _diseaseGroupServiceRepository.GetExistingServiceIdsAsync(request.DiseaseGroupId);
                var newServiceIds = request.ServiceIds.Where(id => !existingServiceIds.Contains(id)).ToList();

                var diseaseGroupServices = newServiceIds.Select(serviceId => new DiseaseGroupService
                {
                    DiseaseGroupId = request.DiseaseGroupId,
                    ServiceId = serviceId
                });

                await _diseaseGroupServiceRepository.AddRangeAsync(diseaseGroupServices);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Added {Count} services to disease group {DiseaseGroupId}", newServiceIds.Count, request.DiseaseGroupId);
                return new AddServicesToDiseaseGroupResult(request.DiseaseGroupId, newServiceIds.Count);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while adding services to disease group {DiseaseGroupId}", request.DiseaseGroupId);
                throw;
            }
        }
    }
}
