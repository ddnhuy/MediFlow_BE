using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.ReceptionVaccinationContracts.Queries
{
    public record GetContractsByCurrentDateQuery : IQuery<List<ContractResponse>>;
    public class GetContractsByCurrentDateQueryHandler : IQueryHandler<GetContractsByCurrentDateQuery, List<ContractResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetContractsByCurrentDateQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContractResponse>> Handle(GetContractsByCurrentDateQuery request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.Date;

            var contracts = await _context.Contracts
                .Where(c => !c.IsCancelled && !c.IsSuspended)
                .Where(c => (c.ExpectedDate.HasValue && c.ExpectedDate.Value.Date == currentDate) && c.Status == ContractStatus.Active)
                .OrderByDescending(c => c.ContractDate)
                .Select(c => new ContractResponse
                {
                    Id = c.Id,
                    ContractCode = c.ContractCode,
                    ContractNumber = c.ContractNumber,
                    ContractName = c.ContractName,
                    CompanyName = c.CompanyName,
                    UnitName = c.UnitName,
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
                    ExpectedPatientCount = c.ExpectedPatientCount,
                    Status = c.Status
                })
                .ToListAsync(cancellationToken);

            return contracts;
        }
    }
}