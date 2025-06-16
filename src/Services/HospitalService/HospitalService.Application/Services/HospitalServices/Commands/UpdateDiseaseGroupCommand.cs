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
    public record UpdateDiseaseGroupCommand(
            int Id,
            string GroupName,
            string? Description
        ) : ICommand<UpdateDiseaseGroupResult>;
    public record UpdateDiseaseGroupResult(int DiseaseGroupId);
    public class UpdateDiseaseGroupCommandHandler : ICommandHandler<UpdateDiseaseGroupCommand, UpdateDiseaseGroupResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateDiseaseGroupCommand> _logger;

        public UpdateDiseaseGroupCommandHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateDiseaseGroupCommand> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UpdateDiseaseGroupResult> Handle(UpdateDiseaseGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var diseaseGroup = await _diseaseGroupRepository.GetByIdAsync(request.Id);
                if (diseaseGroup == null)
                    throw new NotFoundException($"DiseaseGroup with ID {request.Id} not found");

                diseaseGroup.GroupName = request.GroupName;
                diseaseGroup.Description = request.Description;

                await _diseaseGroupRepository.UpdateAsync(diseaseGroup);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Updated disease group {DiseaseGroupId}", request.Id);
                return new UpdateDiseaseGroupResult(request.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while updating disease group {DiseaseGroupId}", request.Id);
                throw;
            }
        }
    }
}