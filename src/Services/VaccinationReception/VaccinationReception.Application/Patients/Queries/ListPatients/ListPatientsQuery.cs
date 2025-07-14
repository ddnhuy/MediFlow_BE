using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReception.Application.Patients.Queries.ListPatients
{
    public record ListPatientsQuery(
        PaginationRequest PaginationRequest,
        string? Name,
        string? Code,
        string? IdentityCard,
        string? PhoneNumber
    ) : IQuery<ListPatientsResult>;
    public record ListPatientsResult(PaginatedResult<PatientSummaryDTO> Patients);
}