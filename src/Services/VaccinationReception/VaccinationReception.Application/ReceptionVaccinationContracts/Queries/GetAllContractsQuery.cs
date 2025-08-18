using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Queries
{
    public record GetAllContractsQuery(
            PaginationRequest PaginationRequest,
            string? SearchTerm
        ) : IQuery<PaginatedResult<ContractResponse>>;

    public class GetAllContractsQueryHandler : IQueryHandler<GetAllContractsQuery, PaginatedResult<ContractResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllContractsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<ContractResponse>> Handle(GetAllContractsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Contracts.AsQueryable();

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.ContractCode.ToLower().Contains(searchTerm) ||
                    c.ContractName.ToLower().Contains(searchTerm) ||
                    c.CompanyName.ToLower().Contains(searchTerm) ||
                    c.UnitName.ToLower().Contains(searchTerm) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm))
                );
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var contracts = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.PaginationRequest.PageIndex - 1) * request.PaginationRequest.PageSize)
                .Take(request.PaginationRequest.PageSize)
                .Select(c => new ContractResponse
                {
                    Id = c.Id,
                    ContractCode = c.ContractCode,
                    ContractNumber = c.ContractNumber,
                    ContractName = c.ContractName,
                    CompanyName = c.CompanyName,
                    UnitName = c.UnitName,
                    Status = c.Status,
                    ContractDate = c.ContractDate,
                    ExpectedDate = c.ExpectedDate,
                    ContractValue = c.ContractValue,
                    AdvanceAmount = c.AdvanceAmount,
                    ActualAmount = c.ActualAmount,
                    Description = c.Description,
                    FileContractId = c.FileContractId,
                    FileContractName = c.FileContractName,
                    FileVaccinationEnrollmentId = c.FileVaccinationEnrollmentId,
                    FileVaccinationEnrollmentName = c.FileVaccinationEnrollmentName,
                    ExpectedPatientCount = c.ExpectedPatientCount
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<ContractResponse>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                totalItems,
                contracts
            );
        }
    }
}
