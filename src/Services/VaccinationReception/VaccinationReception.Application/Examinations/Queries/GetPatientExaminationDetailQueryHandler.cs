using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetPatientExaminationDetailQueryHandler : IQueryHandler<GetPatientExaminationDetailQuery, GetPatientExaminationDetailQueryResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetPatientExaminationDetailQueryHandler> _logger;

        public GetPatientExaminationDetailQueryHandler(
            IApplicationDbContext context,
            ILogger<GetPatientExaminationDetailQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetPatientExaminationDetailQueryResponse> Handle(GetPatientExaminationDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting examination details for ExaminationId: {ExaminationId}",
                request.ExaminationId);

            try
            {
                var examination = await _context.Examinations
                    .FirstOrDefaultAsync(e => e.Id == request.ExaminationId && !e.IsCancelled && !e.IsSuspended, cancellationToken);

                if (examination == null)
                {
                    _logger.LogWarning("Examination not found with Id: {ExaminationId}", request.ExaminationId);
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_EXAMINATION_WITH_ID);
                }

                _logger.LogInformation("Found examination with Id: {ExaminationId}", request.ExaminationId);

                // Return examination details with null values converted to empty strings
                return new GetPatientExaminationDetailQueryResponse
                {
                    Diagnose = examination.Diagnose ?? string.Empty,
                    ReceptionTime = examination.ReceptionTime,
                    PerformTechnicianId = examination.PerformTechnicianId,
                    PerformTechnicianName = examination.PerformTechnicianName ?? string.Empty,
                    SampleType = examination.SampleType,
                    SampleQuality = examination.SampleQuality,
                    ExecutionTime = examination.ExecutionTime,
                    DoctorId = examination.DoctorId,
                    DoctorName = examination.DoctorName ?? string.Empty,
                    Conclusion = examination.Conclusion ?? string.Empty,
                    Note = examination.Note ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting examination details for ExaminationId: {ExaminationId}", request.ExaminationId);
                throw;
            }
        }
    }
}
