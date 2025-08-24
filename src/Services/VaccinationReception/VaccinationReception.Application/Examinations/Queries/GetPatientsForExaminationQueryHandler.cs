using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;
using VaccinationReception.Domain.Enums;

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
                    .Where(e => string.IsNullOrEmpty(e.Diagnose) && !e.ReturnTime.HasValue && string.IsNullOrEmpty(e.Conclusion)) // Not take examinations with diagnose, return time or conclusion
                    .Where(e => !e.IsCancelled && !e.IsSuspended);

                // Get all examinations with their reception and check payment status
                var examinationsWithPaymentStatus = await query
                    .Join(_context.ServiceRequestDetails,
                        e => new { e.ReceptionId, e.ServiceId },
                        srd => new { ReceptionId = (int?)srd.ReceptionId, ServiceId = (int?)srd.ServiceId },
                        (e, srd) => new { Examination = e, ServiceDetail = srd })
                    .Where(joined => joined.ServiceDetail.PaymentStatus == PaymentStatusForItem.Paid) // Only paid examinations
                    .Select(joined => new
                    {
                        joined.Examination.Id,
                        joined.Examination.ReceptionId,
                        joined.Examination.PatientId,
                        joined.Examination.ReceptionTime,
                        joined.Examination.ExecutionTime,
                        joined.Examination.ReturnTime,
                        joined.Examination.RequestNumber,
                        joined.Examination.Diagnose,
                        joined.Examination.Conclusion,
                        joined.Examination.Note
                    })
                    .ToListAsync(cancellationToken);

                if (!examinationsWithPaymentStatus.Any())
                {
                    _logger.LogInformation("No paid examination records found.");
                    return new GetPatientsForExaminationResponse(new List<PatientExaminationInfo>());
                }

                // Get unique patient IDs from examinations
                var patientIds = examinationsWithPaymentStatus
                    .Where(e => e.PatientId.HasValue)
                    .Select(e => e.PatientId.Value)
                    .Distinct()
                    .ToList();

                _logger.LogInformation("Found {Count} unique patients with paid examination records.", patientIds.Count);

                // Get patient information from CustomerInfo service
                var patients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(
                    patientIds,
                    null,
                    cancellationToken);

                patients = patients.Where(p => p.Name.Contains(request.PatientName!)).ToList();

                // Create a dictionary for quick patient lookup
                var patientDictionary = patients.ToDictionary(p => p.Id, p => p);

                // Build the response with examination and patient information
                var patientExaminationInfos = new List<PatientExaminationInfo>();

                // Group by ReceptionId and PatientId to avoid duplicates
                var groupedExaminations = examinationsWithPaymentStatus
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

                _logger.LogInformation("Returning {Count} patient examination records with paid status.", patientExaminationInfos.Count);

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