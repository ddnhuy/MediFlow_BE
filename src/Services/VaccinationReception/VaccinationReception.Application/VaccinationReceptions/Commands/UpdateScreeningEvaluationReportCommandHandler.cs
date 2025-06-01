using BuildingBlocks.CQRS;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class UpdateScreeningEvaluationReportCommandHandler : ICommandHandler<UpdateScreeningEvaluationReportCommand, UpdateScreeningEvaluationReportResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateScreeningEvaluationReportCommandHandler> _logger;

        public UpdateScreeningEvaluationReportCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateScreeningEvaluationReportCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateScreeningEvaluationReportResult> Handle(UpdateScreeningEvaluationReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.ScreeningEvaluationReports.FindAsync(new object[] { request.Id }, cancellationToken);

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