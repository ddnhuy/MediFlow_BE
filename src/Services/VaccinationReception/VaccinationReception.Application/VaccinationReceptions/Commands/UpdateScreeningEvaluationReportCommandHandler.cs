using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class UpdateScreeningEvaluationReportCommandHandler : ICommandHandler<UpdateScreeningEvaluationReportCommand, UpdateScreeningEvaluationReportResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateScreeningEvaluationReportCommandHandler> _logger;

        public UpdateScreeningEvaluationReportCommandHandler(
            IApplicationDbContext context,
            ILogger<UpdateScreeningEvaluationReportCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateScreeningEvaluationReportResult> Handle(UpdateScreeningEvaluationReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ReceptionId <= 0)
                {
                    _logger.LogWarning("Invalid ReceptionId: {ReceptionId}", request.ReceptionId);
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionId && !rv.IsCancelled, cancellationToken);

                if (reception == null)
                {
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
                }

                reception.LastUpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                var entity = await _context.ScreeningEvaluationReports.FindAsync(request.Id, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("ScreeningEvaluationReport with Id {Id} not found", request.Id);
                    return new UpdateScreeningEvaluationReportResult(false);
                }

                request.Adapt(entity);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated ScreeningEvaluationReport with Id = {Id}", request.Id);

                return new UpdateScreeningEvaluationReportResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling UpdateScreeningEvaluationReportCommand. Id = {Id}", request.Id);
                throw;
            }
        }
    }
}