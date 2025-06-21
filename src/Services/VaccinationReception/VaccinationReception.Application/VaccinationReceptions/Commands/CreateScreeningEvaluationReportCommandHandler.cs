using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreateScreeningEvaluationReportHandler : IRequestHandler<CreateScreeningEvaluationReportCommand, CreateScreeningEvaluationReportResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateScreeningEvaluationReportHandler> _logger;

        public CreateScreeningEvaluationReportHandler(IApplicationDbContext context, ILogger<CreateScreeningEvaluationReportHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateScreeningEvaluationReportResult> Handle(CreateScreeningEvaluationReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ReceptionId <= 0)
                {
                    _logger.LogWarning("Invalid ReceptionId: {ReceptionId}", request.ReceptionId);
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var entity = request.Adapt<ScreeningEvaluationReport>();
                var entityEntry = await _context.ScreeningEvaluationReports.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created ScreeningEvaluationReport with Id = {Id}", entityEntry.Entity.Id);

                return new CreateScreeningEvaluationReportResult(entityEntry.Entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling CreateScreeningEvaluationReportCommand. ReceptionId = {ReceptionId}", request.ReceptionId);
                throw;
            }
        }
    }
}