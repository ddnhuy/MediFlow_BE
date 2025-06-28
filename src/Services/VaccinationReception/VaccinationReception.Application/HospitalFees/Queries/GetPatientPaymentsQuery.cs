using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.HospitalFeeDTOs;

namespace VaccinationReception.Application.HospitalFees.Queries
{
    public record GetPatientPaymentsQuery(int PatientId) : IQuery<PatientPaymentsResponseDTO>;

    public class GetPatientPaymentsQueryHandler : IQueryHandler<GetPatientPaymentsQuery, PatientPaymentsResponseDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetPatientPaymentsQueryHandler> _logger;

        public GetPatientPaymentsQueryHandler(IApplicationDbContext context, ILogger<GetPatientPaymentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PatientPaymentsResponseDTO> Handle(GetPatientPaymentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting payments for PatientId: {PatientId}", request.PatientId);

            var payments = await _context.Payments
                .Include(p => p.Reception)
                .Where(p => p.Reception.PatientId == request.PatientId && !p.IsCancelled)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentDTO(
                    p.Id,
                    p.ReceptionId,
                    p.TotalAmount,
                    p.Method,
                    p.Note,
                    p.ATMTransactionCode,
                    p.PaymentType,
                    p.InvoiceNumber,
                    p.Status,
                    p.OriginalPaymentId,
                    p.CreatedAt,
                    p.LastUpdatedAt
                ))
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} payments for PatientId: {PatientId}", payments.Count, request.PatientId);

            return new PatientPaymentsResponseDTO(request.PatientId, payments);
        }
    }
}
