using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Patients.Commands.DeletePatient
{
    public class DeletePatientCommandValidator: AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Vui lòng nhập Id");
        }
    }
}