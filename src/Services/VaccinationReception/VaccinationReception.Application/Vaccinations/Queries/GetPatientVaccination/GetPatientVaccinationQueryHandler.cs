using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.Vaccinations.Queries.GetPatientVaccination
{
    public class GetPatientVaccinationQueryHandler : IQueryHandler<GetPatientVaccinationQuery, GetPatientVaccinationQueryResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetPatientVaccinationQueryHandler> _logger;

        public GetPatientVaccinationQueryHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetPatientVaccinationQueryHandler> logger)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<GetPatientVaccinationQueryResult> Handle(GetPatientVaccinationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving all reception vaccinations with paid payment status");

                // Get all paid, not-cancelled reception vaccinations, including Reception and ScreeningEvaluationReport
                var paidReceptionVaccinations = await _context.ReceptionVaccinations
                    .Include(rv => rv.Reception)
                    .ThenInclude(r => r.ScreeningEvaluationReport)
                    .Where(rv => rv.Reception.IsVaccinationTodayConfirmed == false)
                    .Where(rv => rv.PaymentStatus == PaymentStatusForItem.Paid && !rv.IsCancelled)
                    .ToListAsync(cancellationToken);

                // Get all ReceptionVaccinationIds
                var paidReceptionVaccinationIds = paidReceptionVaccinations.Select(rv => rv.Id).ToList();

                // Get all Vaccinations for these ReceptionVaccinations
                var vaccinations = await _context.Vaccinations
                    .Where(v => paidReceptionVaccinationIds.Contains(v.ReceptionVaccinationId))
                    .ToListAsync(cancellationToken);

                // Find ReceptionVaccinations where number of Vaccination records <= Quantity
                var pendingReceptionVaccinations = paidReceptionVaccinations
                    .Where(rv =>
                    {
                        var relatedVaccinations = vaccinations.Where(v => v.ReceptionVaccinationId == rv.Id).ToList();
                        return relatedVaccinations.Count <= rv.Quantity;
                    }).ToList();

                // Group by ReceptionId and select the first for each Reception
                var receptionsWithPendingVaccinations = pendingReceptionVaccinations
                    .GroupBy(rv => rv.Reception.Id)
                    .Select(group => group.First())
                    .OrderBy(rv => rv.Reception.ReceptionDate)
                    .ToList();

                _logger.LogInformation("Found {Count} receptions with pending vaccinations", receptionsWithPendingVaccinations.Count);

                var patientVaccinationItems = new List<PatientVaccinationItem>();

                foreach (var receptionVaccination in receptionsWithPendingVaccinations)
                {
                    try
                    {
                        // Get patient information from the gRPC service
                        var patient = await _patientGrpcClient.GetPatientAsync(receptionVaccination.Reception.PatientId, cancellationToken);

                        var weightKg = receptionVaccination.Reception.ScreeningEvaluationReport?.WeightKg ?? 0.0;

                        var genderString = patient.Gender == 0 ? "Nữ" : "Nam";

                        var patientVaccinationItem = new PatientVaccinationItem(
                            ReceptionId: receptionVaccination.Reception.Id,
                            PatientId: receptionVaccination.Reception.PatientId,
                            PatientCode: patient.Code,
                            PatientVaccinationCode: "Todo: Handle later",
                            PatientName: patient.Name,
                            DateOfBirth: patient.DOB,
                            Gender: genderString,
                            WeightKg: weightKg
                        );

                        patientVaccinationItems.Add(patientVaccinationItem);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to retrieve patient information for PatientId: {PatientId}, ReceptionVaccinationId: {ReceptionVaccinationId}",
                            receptionVaccination.Reception.PatientId, receptionVaccination.Id);
                    }
                }

                _logger.LogInformation("Successfully processed {Count} patient vaccination items", patientVaccinationItems.Count);

                return new GetPatientVaccinationQueryResult(patientVaccinationItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving patient vaccinations with paid status");
                throw;
            }
        }
    }
}