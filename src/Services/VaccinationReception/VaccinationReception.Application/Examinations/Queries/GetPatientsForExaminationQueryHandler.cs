using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetPatientsForExaminationQueryHandler : IQueryHandler<GetPatientsForExaminationQuery, GetPatientsForExaminationResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetPatientsForExaminationQueryHandler> _logger;

        public GetPatientsForExaminationQueryHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetPatientsForExaminationQueryHandler> logger)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<GetPatientsForExaminationResponse> Handle(GetPatientsForExaminationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting patients with examination records. PatientName filter: {PatientName}", request.PatientName);

            try
            {
                var query = _context.Examinations
                    .Where(e => !e.IsCancelled && !e.IsSuspended);

                if (request.IsDiagnose.HasValue)
                {
                    if (request.IsDiagnose.Value == true)
                    {
                        query = query.Where(e => !string.IsNullOrEmpty(e.Diagnose));
                    }
                    else
                    {
                        query = query.Where(e => string.IsNullOrEmpty(e.Diagnose));
                    }
                }

                // Get all examinations with their reception and patient information
                var examinationsWithPatients = await query
                    .Select(e => new
                    {
                        e.Id,
                        e.ReceptionId,
                        e.PatientId,
                        e.ReceptionTime,
                        e.ExecutionTime,
                        e.ReturnTime,
                        e.RequestNumber,
                        e.Diagnose,
                        e.Conclusion,
                        e.Note
                    })
                    .ToListAsync(cancellationToken);

                if (!examinationsWithPatients.Any())
                {
                    _logger.LogInformation("No examination records found.");
                    return new GetPatientsForExaminationResponse(new List<PatientExaminationInfo>());
                }

                // Get unique patient IDs from examinations
                var patientIds = examinationsWithPatients
                    .Where(e => e.PatientId.HasValue)
                    .Select(e => e.PatientId.Value)
                    .Distinct()
                    .ToList();

                _logger.LogInformation("Found {Count} unique patients with examination records.", patientIds.Count);

                // Get patient information from CustomerInfo service
                var patients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(
                    patientIds,
                    request.PatientName,
                    cancellationToken);

                // Create a dictionary for quick patient lookup
                var patientDictionary = patients.ToDictionary(p => p.Id, p => p);

                // Build the response with examination and patient information
                var patientExaminationInfos = new List<PatientExaminationInfo>();

                // Group by ReceptionId and PatientId to avoid duplicates
                var groupedExaminations = examinationsWithPatients
                    .Where(e => e.PatientId.HasValue)
                    .GroupBy(e => new { e.ReceptionId, e.PatientId })
                    .Select(g => g.First()) // Take the first examination for each reception-patient combination
                    .ToList();

                foreach (var examination in groupedExaminations)
                {
                    if (patientDictionary.TryGetValue(examination.PatientId.Value, out var patient))
                    {
                        var age = DateTime.Now.Year - patient.DOB.Year;
                        if (DateTime.Now < patient.DOB.AddYears(age))
                        {
                            age--;
                        }

                        var gender = patient.Gender == 0 ? "Nữ" : "Nam";

                        var patientExaminationInfo = new PatientExaminationInfo(
                            ReceptionId: examination.ReceptionId ?? 0,
                            PatientId: examination.PatientId.Value,
                            PatientName: patient.Name,
                            YearOfBirth: patient.DOB.Year,
                            PatientCode: patient.Code,
                            Age: age,
                            Gender: gender
                        );

                        patientExaminationInfos.Add(patientExaminationInfo);
                    }
                }

                _logger.LogInformation("Returning {Count} patient examination records.", patientExaminationInfos.Count);

                return new GetPatientsForExaminationResponse(patientExaminationInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients with examination records.");
                throw;
            }
        }
    }
}