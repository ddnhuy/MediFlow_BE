using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationPatient
{
    public class GetListPostVaccinationPatientQueryHandler : IQueryHandler<GetListPostVaccinationPatientQuery, List<GetListPostVaccinationPatientQueryResult>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPatientGrpcClient _patientGrpcClient;

        public GetListPostVaccinationPatientQueryHandler(IApplicationDbContext dbContext, IPatientGrpcClient patientGrpcClient)
        {
            _dbContext = dbContext;
            _patientGrpcClient = patientGrpcClient;
        }

        public async Task<List<GetListPostVaccinationPatientQueryResult>> Handle(
            GetListPostVaccinationPatientQuery request,
            CancellationToken cancellationToken)
        {
            var postVaccinationPatient = await _dbContext.Vaccinations
                .Include(v => v.ReceptionVaccination)
                .Where(v => !v.ObservationConfirmed)
                // Filter by PatientVaccinationCode if provided
                //.Where(v => string.IsNullOrEmpty(request.PatientVaccinationCode)
                //    || v.ReceptionVaccination.RequestNumber == request.PatientVaccinationCode)
                .ToListAsync(cancellationToken);

            // Retrieve patients, filtered by PatientName if provided
            var patientsPagination = await _patientGrpcClient
                .ListPatientsAsync(new BuildingBlocks.Pagination.PaginationRequest(PageIndex: 1, PageSize: 999), name: request.PatientName, "", "", "", cancellationToken);

            var patients = patientsPagination.Data ?? new List<PatientSummaryDTO>();   

            // Filter results to matching patient IDs
            var matchingPatientIds = patients.Select(p => p.Id).ToHashSet();
            postVaccinationPatient = postVaccinationPatient
                .Where(v => matchingPatientIds.Contains(v.PatientId))
                .GroupBy(v => v.PatientId)
                .Select(g => g.First())
                .ToList();

            // Build final result
            var result = postVaccinationPatient.Select(v =>
            {
                var patient = patients.FirstOrDefault(p => p.Id == v.PatientId);
                return new GetListPostVaccinationPatientQueryResult
                (
                    ReceptionId: v.ReceptionVaccination!.ReceptionId,
                    PatientVaccinationCode: "",
                    PatientName: patient?.Name ?? string.Empty,
                    YearOfBirth: patient?.DOB.Year > 0 ? new DateOnly(patient.DOB.Year, patient.DOB.Month, patient.DOB.Day) : DateOnly.MinValue,
                    PatientCode: patient?.Code ?? string.Empty,
                    Gender: patient?.Gender == 0 ? "Nữ" : "Nam"
                );
            }).ToList();

            return result;
        }
    }
}
