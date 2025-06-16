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
    public record CreateDiseaseGroupCommand(
        string GroupName,
        string? Description,
        List<int>? ServiceIds
    ) : ICommand<CreateDiseaseGroupResult>;

    public record CreateDiseaseGroupResult(int DiseaseGroupId);

    public class CreateDiseaseGroupCommandHandler : ICommandHandler<CreateDiseaseGroupCommand, CreateDiseaseGroupResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly IDiseaseGroupServiceRepository _diseaseGroupServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateDiseaseGroupCommand> _logger;

        public CreateDiseaseGroupCommandHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            IDiseaseGroupServiceRepository diseaseGroupServiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateDiseaseGroupCommand> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _diseaseGroupServiceRepository = diseaseGroupServiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateDiseaseGroupResult> Handle(CreateDiseaseGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var diseaseGroup = new DiseaseGroup
                {
                    GroupName = request.GroupName,
                    Description = request.Description
                };

                await _diseaseGroupRepository.AddAsync(diseaseGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.ServiceIds != null && request.ServiceIds.Any())
                {
                    var diseaseGroupServices = request.ServiceIds.Select(serviceId => new DiseaseGroupService
                    {
                        DiseaseGroupId = diseaseGroup.Id,
                        ServiceId = serviceId,
                    });

                    await _diseaseGroupServiceRepository.AddRangeAsync(diseaseGroupServices);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Created new disease group with ID {DiseaseGroupId}", diseaseGroup.Id);
                return new CreateDiseaseGroupResult(diseaseGroup.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while creating disease group: {Message}", ex.InnerException?.Message ?? ex.Message);
                throw;
            }
        }
    }
}
