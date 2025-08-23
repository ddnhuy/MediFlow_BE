using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.HospitalFeeDTOs;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.HospitalFees.Queries
{
    public record GetAllPaymentsWithPatientsQuery(
        PaginationRequest PaginationRequest,
        string? SearchTerm,
        DateTime? FromDate,
        DateTime? ToDate
    ) : IQuery<PaginatedResult<PaymentWithPatientDTO>>;

    public class GetAllPaymentsWithPatientsQueryHandler
        : IQueryHandler<GetAllPaymentsWithPatientsQuery, PaginatedResult<PaymentWithPatientDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;

        public GetAllPaymentsWithPatientsQueryHandler(IApplicationDbContext context, IPatientGrpcClient patientGrpcClient)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
        }

        public async Task<PaginatedResult<PaymentWithPatientDTO>> Handle(GetAllPaymentsWithPatientsQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;
            var searchTerm = request.SearchTerm?.Trim();

            var query = _context.Payments
                .Include(p => p.Reception)
                .Where(p => !p.IsCancelled);

            if (request.FromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt < request.ToDate.Value);
            }

            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    Payment = p,
                    PatientId = p.Reception.PatientId
                })
                .ToListAsync(cancellationToken);

            var patientIds = payments.Select(x => x.PatientId).Distinct().ToList();
            var patients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(patientIds, null, cancellationToken);
            var patientDict = patients.ToDictionary(p => p.Id, p => p);

            var fullList = payments
                .Where(x => patientDict.ContainsKey(x.PatientId))
                .Select(x => new PaymentWithPatientDTO(
                    new PaymentDTO(
                        x.Payment.Id,
                        x.Payment.ReceptionId,
                        x.Payment.TotalAmount,
                        x.Payment.Method,
                        x.Payment.Note,
                        x.Payment.ATMTransactionCode,
                        x.Payment.PaymentType,
                        x.Payment.InvoiceNumber,
                        x.Payment.Status,
                        x.Payment.OriginalPaymentId,
                        x.Payment.CreatedAt,
                        x.Payment.LastUpdatedAt
                    ),
                    patientDict[x.PatientId]
                ))
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                fullList = fullList.Where(item =>
                    (item.Patient.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.Patient.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.Patient.IdentityCard?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.Payment.InvoiceNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            var totalCount = fullList.Count;
            var pagedData = fullList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<PaymentWithPatientDTO>(pageIndex, pageSize, totalCount, pagedData);
        }
    }
}