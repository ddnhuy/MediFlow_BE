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
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetScreeningEvaluationReportByReceptionIdQuery(int ReceptionId)
        : IQuery<GetScreeningEvaluationReportByReceptionIdResult>;
    public record GetScreeningEvaluationReportByReceptionIdResult(ScreeningEvaluationReportDTO? Report);
    public class GetScreeningEvaluationReportByReceptionIdQueryHandler
       : IQueryHandler<GetScreeningEvaluationReportByReceptionIdQuery, GetScreeningEvaluationReportByReceptionIdResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetScreeningEvaluationReportByReceptionIdQueryHandler> _logger;

        public GetScreeningEvaluationReportByReceptionIdQueryHandler(
            IApplicationDbContext context,
            ILogger<GetScreeningEvaluationReportByReceptionIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetScreeningEvaluationReportByReceptionIdResult> Handle(
            GetScreeningEvaluationReportByReceptionIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting screening evaluation report for ReceptionId: {ReceptionId}", request.ReceptionId);

                if (request.ReceptionId <= 0)
                {
                    _logger.LogWarning("Invalid ReceptionId: {ReceptionId}", request.ReceptionId);
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == request.ReceptionId && !r.IsCancelled, cancellationToken);

                if (reception == null)
                {
                    _logger.LogWarning("Reception not found or cancelled for ReceptionId: {ReceptionId}", request.ReceptionId);
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
                }

                var report = await _context.ScreeningEvaluationReports
                    .FirstOrDefaultAsync(ser => ser.ReceptionId == request.ReceptionId, cancellationToken);

                ScreeningEvaluationReportDTO? reportDto = null;
                if (report != null)
                {
                    reportDto = report.Adapt<ScreeningEvaluationReportDTO>();
                    _logger.LogInformation("Found screening evaluation report with Id: {ReportId} for ReceptionId: {ReceptionId}",
                        report.Id, request.ReceptionId);
                }
                else
                {
                    _logger.LogInformation("No screening evaluation report found for ReceptionId: {ReceptionId}", request.ReceptionId);
                }

                return new GetScreeningEvaluationReportByReceptionIdResult(reportDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving screening evaluation report for ReceptionId: {ReceptionId}", request.ReceptionId);
                throw;
            }
        }
    }
}
