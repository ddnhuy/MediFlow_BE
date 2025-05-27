using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Patients.Commands.UpdatePatient
{
    public record UpdatePatientCommand(
        int Id,
        string Code,
        string Name,
        int Gender,
        DateTime Dob,
        string PhoneNumber,
        string IdentityCard,
        string AddressDetail,
        string Province,
        string District,
        string Ward,
        bool IsPregnant,
        bool IsForeigner,
        bool IsSuspended,
        bool IsCancelled
    ) : ICommand<UpdatePatientResult>;

    public record UpdatePatientResult(bool IsSuccess);
}