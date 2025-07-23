using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetAllExaminationHistoryQuery(PaginationRequest PaginationRequest, string? searchTerm) : IQuery<GetAllExaminationHistoryResponse>;

    public record GetAllExaminationHistoryResponse(PaginatedResult<ExaminationHistoryDTO> PaginatedResult);

    public class ExaminationHistoryDTO
    {
        public int PatientId { get; set; }
        public string? PatientCode { get; set; } = string.Empty;
        public string? PatientName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public DateTime LastExaminationDate { get; set; }
    }
}
