using BuildingBlocks.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Patients.Commands;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Patients.Commands.DeletePatient;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;

namespace VaccinationReception.Application.Services.PatientServices
{
    public interface IPatientGrpcClient
    {
        Task<PaginatedResult<PatientSummaryDTO>> ListPatientsAsync(PaginationRequest request, string? name, string? code, string? identityCard, string? phoneNumber, CancellationToken cancellationToken);
        Task<PatientDetailDTO> GetPatientAsync(int id, CancellationToken cancellationToken);
        Task<CreatePatientResult> CreatePatientAsync(CreatePatientCommand command, CancellationToken cancellationToken);
        Task<UpdatePatientResult> UpdatePatientAsync(UpdatePatientCommand command, CancellationToken cancellationToken);
        Task<DeletePatientResult> DeletePatientAsync(int id, CancellationToken cancellationToken);
        Task<List<PatientSummaryDTO>> ListPatientsByIdsAndSearchAsync(List<int> ids, string? searchTerm, CancellationToken cancellationToken);
    }
}