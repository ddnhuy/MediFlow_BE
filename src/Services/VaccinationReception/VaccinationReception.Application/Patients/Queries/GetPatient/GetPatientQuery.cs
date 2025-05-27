using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReception.Application.Patients.Queries.GetPatient
{
    public record GetPatientQuery(int Id) : IQuery<GetPatientResult>;
    public record GetPatientResult(PatientDetailDTO Patient);
}