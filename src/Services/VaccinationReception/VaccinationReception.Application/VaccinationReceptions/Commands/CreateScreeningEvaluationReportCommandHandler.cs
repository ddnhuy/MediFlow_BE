using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using BuildingBlocks.Strings.Consts.HospitalServices;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CreateScreeningEvaluationReportHandler : IRequestHandler<CreateScreeningEvaluationReportCommand, CreateScreeningEvaluationReportResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateScreeningEvaluationReportHandler> _logger;
        private readonly IHospitalService _hospitalService;

        public CreateScreeningEvaluationReportHandler(IApplicationDbContext context, ILogger<CreateScreeningEvaluationReportHandler> logger, IHospitalService hospitalService)
        {
            _context = context;
            _logger = logger;
            _hospitalService = hospitalService;
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

                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionId && !rv.IsCancelled, cancellationToken);

                if (reception == null)
                {
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
                }

                reception.LastUpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                var existingReport = await _context.ScreeningEvaluationReports
                    .FirstOrDefaultAsync(report => report.ReceptionId == request.ReceptionId, cancellationToken);

                if (existingReport != null)
                {
                    throw new BadRequestException(ExceptionKey.GENERATED_RESULTS_DO_NOT_REPRODUCE);
                }

                var serviceRequest = await _hospitalService.GetServicesByServiceCodeAsync(
                  new List<string> { ServiceCodeConsts.EXAM_FEE_SERVICE_CODE }, cancellationToken);

                var serviceExamFee = serviceRequest.FirstOrDefault(m => m.ServiceCode == ServiceCodeConsts.EXAM_FEE_SERVICE_CODE);

                if (serviceExamFee == null)
                {
                    _logger.LogWarning("Exam fee service not found for ReceptionId = {ReceptionId}", request.ReceptionId);
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_EXAM_FEE);
                }

                var examFeeServiceDetail = await _context.ServiceRequestDetails
                    .Where(srd => srd.ReceptionId == request.ReceptionId && srd.ServiceId == serviceExamFee.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (examFeeServiceDetail != null && examFeeServiceDetail.PaymentStatus != PaymentStatusForItem.Paid)
                {
                    _logger.LogWarning("Exam fee service not paid for ReceptionId = {ReceptionId}", request.ReceptionId);
                    throw new BadRequestException(ExceptionKey.UNPAID_EXAM_FEE);
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