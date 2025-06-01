using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Patients.Commands.CreatePatient;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record CreatePatientReceptionCommand(
        CreatePatientCommand createPatientCommand, CreateReceptionDTO createReceptionDTO, int patientId
     ) : ICommand<CreatePatientReceptionResult>;

    public record CreatePatientReceptionResult(int patientId, int receptionId);
}