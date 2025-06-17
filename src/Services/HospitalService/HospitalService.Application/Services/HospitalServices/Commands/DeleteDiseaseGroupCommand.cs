using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
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
    public record DeleteDiseaseGroupCommand(
            int Id
        ) : ICommand<DeleteDiseaseGroupResult>;
    public record DeleteDiseaseGroupResult(int DiseaseGroupId);

    public class DeleteDiseaseGroupCommandHandler : ICommandHandler<DeleteDiseaseGroupCommand, DeleteDiseaseGroupResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteDiseaseGroupCommand> _logger;

        public DeleteDiseaseGroupCommandHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteDiseaseGroupCommand> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DeleteDiseaseGroupResult> Handle(DeleteDiseaseGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var diseaseGroup = await _diseaseGroupRepository.GetByIdAsync(request.Id);
                if (diseaseGroup == null)
                    throw new NotFoundException($"DiseaseGroup with ID {request.Id} not found");

                diseaseGroup.IsCancelled = true;

                var diseaseGroupServices = await _diseaseGroupServiceRepository.GetByServiceGroupIdAsync(request.Id);
                await _diseaseGroupServiceRepository.DeleteRangeAsync(diseaseGroupServices);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Deleted disease group {DiseaseGroupId}", request.Id);
                return new DeleteDiseaseGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while deleting disease group {DiseaseGroupId}", request.Id);
                throw;
            }
        }
    }
}
