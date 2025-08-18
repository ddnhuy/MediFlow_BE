using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Handlers
{
    public record UpdateContractStatusCommand(
            int ContractId,
            ContractStatus Status,
            string? Reason = null
        ) : ICommand<bool>;

    public class UpdateContractStatusCommandHandler : ICommandHandler<UpdateContractStatusCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateContractStatusCommandHandler> _logger;

        public UpdateContractStatusCommandHandler(
            IApplicationDbContext context,
            ILogger<UpdateContractStatusCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateContractStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(x => x.Id == request.ContractId, cancellationToken);

                if (contract == null)
                {
                    _logger.LogWarning("Contract with Id: {ContractId} not found", request.ContractId);
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);
                }

                if (request.Status == ContractStatus.Cancelled)
                {
                    if (contract.Status == ContractStatus.Completed || contract.Status == ContractStatus.Finalized)
                    {
                        _logger.LogWarning("Cannot cancel contract {ContractId} with current status: {Status}",
                            request.ContractId, contract.Status);
                        throw new BadRequestException(ExceptionKey.CANNOT_CANCEL_CONTRACT);
                    }
                }

                var oldStatus = contract.Status;

                contract.Status = request.Status;

                if (!string.IsNullOrEmpty(request.Reason))
                {
                    contract.Description = request.Reason;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated contract {ContractId} status from {OldStatus} to {NewStatus}",
                    request.ContractId, oldStatus, request.Status);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract {ContractId} status to {Status}",
                    request.ContractId, request.Status);
                throw;
            }
        }
    }
}
