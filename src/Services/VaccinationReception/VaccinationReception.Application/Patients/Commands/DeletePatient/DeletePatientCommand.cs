using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Patients.Commands.DeletePatient
{
    public record DeletePatientCommand(int Id) : ICommand<DeletePatientResult>;
    public record DeletePatientResult(bool IsSuccess);
}