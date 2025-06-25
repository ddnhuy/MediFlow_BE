using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.Patients.Queries.ListPatients
{
    public record GetListPatientsUnpaidServiceQuery(
        string? SearchTerm
    ) : IQuery<ListPatientsUnpaidServiceQueryResult>;

    public record ListPatientsUnpaidServiceQueryResult(IEnumerable<PatientDTO> Patients);

    public class GetListPatientsUnpaidServiceQueryHandler
        : IQueryHandler<GetListPatientsUnpaidServiceQuery, ListPatientsUnpaidServiceQueryResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetListPatientsUnpaidServiceQueryHandler> _logger;

        public GetListPatientsUnpaidServiceQueryHandler(
            IApplicationDbContext dbContext,
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetListPatientsUnpaidServiceQueryHandler> logger)
        {
            _dbContext = dbContext;
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }
        public async Task<ListPatientsUnpaidServiceQueryResult> Handle(GetListPatientsUnpaidServiceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start fetching patients with unpaid services.");

            var patientIdsFromVaccinations = await _dbContext.ReceptionVaccinations
                .Where(rv => rv.PaymentStatus == PaymentStatusForItem.NotPaid && !rv.IsCancelled)
                .Select(rv => rv.Reception.PatientId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var patientIdsFromServiceRequests = await _dbContext.ServiceRequestDetails
                .Where(srd => srd.PaymentStatus == PaymentStatusForItem.NotPaid && !srd.IsCancelled)
                .Select(srd => srd.RequestForm.Reception.PatientId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var allPatientIds = patientIdsFromVaccinations
                .Concat(patientIdsFromServiceRequests)
                .Distinct()
                .ToList();

            if (!allPatientIds.Any())
            {
                _logger.LogInformation("No patients with unpaid services found.");
                return new ListPatientsUnpaidServiceQueryResult(Enumerable.Empty<PatientDTO>());
            }

            _logger.LogInformation("Found {Count} distinct patients with unpaid services.", allPatientIds.Count);

            var patients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(allPatientIds, request.SearchTerm, cancellationToken);

            var patientDtos = patients
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    DOB = p.DOB
                })
                .ToList();

            _logger.LogInformation("Returning {Count} patients after filtering.", patientDtos.Count);

            return new ListPatientsUnpaidServiceQueryResult(patientDtos);
        }
    }
}
