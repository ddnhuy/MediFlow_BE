using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Strings;
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

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Queries
{
    public record GetPatientsByContractIdQuery (
        int ContractId,
        PaginationRequest PaginationRequest,
        string SearchTerm
    ) : IQuery<PaginatedResult<PatientSummaryDTO>>;
    public class GetPatientsByContractIdQueryHandler : IQueryHandler<GetPatientsByContractIdQuery, PaginatedResult<PatientSummaryDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetPatientsByContractIdQueryHandler> _logger;

        public GetPatientsByContractIdQueryHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetPatientsByContractIdQueryHandler> logger)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<PaginatedResult<PatientSummaryDTO>> Handle(GetPatientsByContractIdQuery request, CancellationToken cancellationToken)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == request.ContractId && !c.IsCancelled && !c.IsSuspended, cancellationToken);

            if (contract == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_CONTRACT_WITH_ID);
            }

            var contractPatientVaccinations = await _context.ContractPatientVaccinations
                .Where(cpv => cpv.ContractId == request.ContractId && !cpv.IsCancelled && !cpv.IsSuspended)
                .ToListAsync(cancellationToken);

            var patientIds = contractPatientVaccinations
                .Select(cpv => cpv.PatientId)
                .Distinct()
                .ToList();

            var allPatients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(patientIds, null, cancellationToken);

            var filtered = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? allPatients
                : allPatients.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.IdentityCard) && p.IdentityCard.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();

            var totalItems = filtered.Count;
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var paginated = filtered
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<PatientSummaryDTO>(pageIndex, pageSize, totalItems, paginated);
        }
    }
}
