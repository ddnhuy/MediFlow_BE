using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class RemoveServicesFromRequestFormCommandHandler : ICommandHandler<RemoveServicesFromRequestFormCommand, RemoveServicesFromRequestFormResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<RemoveServicesFromRequestFormCommand> _logger;

        public RemoveServicesFromRequestFormCommandHandler(
            IApplicationDbContext context,
            ILogger<RemoveServicesFromRequestFormCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RemoveServicesFromRequestFormResult> Handle(RemoveServicesFromRequestFormCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

                if (reception == null)
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);

                var serviceRequestDetails = await _context.ServiceRequestDetails
                    .Where(srd => srd.ReceptionId == reception.Id && request.ServiceIds.Contains(srd.ServiceId))
                    .ToListAsync(cancellationToken);

                if (!serviceRequestDetails.Any())
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_SERVICE_REQUEST);

                foreach (var detail in serviceRequestDetails)
                {
                    detail.IsCancelled = true;
                    detail.LastUpdatedAt = DateTime.UtcNow;
                }

                var activeServices = await _context.ServiceRequestDetails
                    .AnyAsync(srd => srd.ReceptionId == reception.Id && !srd.IsCancelled, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new RemoveServicesFromRequestFormResult(reception.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling RemoveServicesFromRequestFormCommand");
                throw;
            }
        }
    }
}