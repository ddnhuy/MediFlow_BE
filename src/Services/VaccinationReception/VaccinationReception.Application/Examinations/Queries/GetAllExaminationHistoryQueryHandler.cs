using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetAllExaminationHistoryQueryHandler : IQueryHandler<GetAllExaminationHistoryQuery, GetAllExaminationHistoryResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;

        public GetAllExaminationHistoryQueryHandler(IApplicationDbContext context, IPatientGrpcClient patientGrpcClient)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
        }

        public async Task<GetAllExaminationHistoryResponse> Handle(GetAllExaminationHistoryQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var patientIDs = await _context.Examinations
                 .Select(e => e.PatientId)
                 .Distinct()
                 .ToListAsync(cancellationToken);

            var examinationHistories = new List<ExaminationHistoryDTO>();

            foreach (var id in patientIDs)
            {
                var patient = await _patientGrpcClient.GetPatientAsync(id!.Value, cancellationToken);

                if (patient == null)
                    continue;

                if (!string.IsNullOrEmpty(request.searchTerm))
                {
                    var term = request.searchTerm;
                    if (!(patient.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
                          patient.Code?.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
                          patient.PhoneNumber?.Contains(term, StringComparison.OrdinalIgnoreCase) == true))
                    {
                        continue;
                    }
                }

                // Get the last examination date for this patient
                var lastExamDate = await _context.Examinations
                    .Where(e => e.PatientId == id)
                    .OrderByDescending(e => e.ReturnTime)
                    .Select(e => e.ReturnTime)
                    .FirstOrDefaultAsync(cancellationToken);

                examinationHistories.Add(new ExaminationHistoryDTO
                {
                    PatientId = id.Value,
                    PatientCode = patient.Code,
                    PatientName = patient.Name,
                    PhoneNumber = patient.PhoneNumber,
                    LastExaminationDate = lastExamDate.Value
                });
            }

            // Paginate the results
            var totalCount = examinationHistories.Count;
            var pagedData = examinationHistories
                .OrderByDescending(e => e.LastExaminationDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<ExaminationHistoryDTO>(
                pageIndex,
                pageSize,
                totalCount,
                pagedData
            );

            return new GetAllExaminationHistoryResponse(paginatedResult);
        }
    }
}
