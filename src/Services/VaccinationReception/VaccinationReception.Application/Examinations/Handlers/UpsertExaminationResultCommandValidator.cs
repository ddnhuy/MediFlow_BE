using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.Examinations.Handlers
{
    public class UpsertExaminationResultCommandValidator : AbstractValidator<UpsertExaminationResultCommand>
    {
        public UpsertExaminationResultCommandValidator()
        {
            RuleFor(x => x.Results)
                .NotNull()
                .NotEmpty();

            RuleForEach(x => x.Results).SetValidator(new ExaminationTestResultUpsertDTOValidator());
        }
    }

    public class ExaminationTestResultUpsertDTOValidator : AbstractValidator<ExaminationTestResultUpsertDTO>
    {
        public ExaminationTestResultUpsertDTOValidator()
        {
            RuleFor(x => x.ExaminationId)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_ID.ToString());
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_ID.ToString());
            RuleFor(x => x.Diagnose)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_DIAGNOSE.ToString());
            RuleFor(x => x.ReturnTime)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_RETURN_TIME.ToString());
            RuleFor(x => x.PerformTechnicianId)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_TECHNICIAN.ToString());
            RuleFor(x => x.SampleType).IsInEnum();
            RuleFor(x => x.SampleQuality).IsInEnum();
            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage(ExceptionKey.INVALID_DOCTOR_ID.ToString()); ;
            RuleFor(x => x.Conclusion)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_CONCLUSION.ToString());
            RuleFor(x => x.ExaminationResults)
                .NotNull()
                .NotEmpty();

            RuleForEach(x => x.ExaminationResults).SetValidator(new ExaminationResultItemValidator());
        }
    }

    public class ExaminationResultItemValidator : AbstractValidator<ExaminationResultItem>
    {
        public ExaminationResultItemValidator()
        {
            RuleFor(x => x.ParameterName)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_PARAMETER_NAME.ToString());
            RuleFor(x => x.ResultValue)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_RESULT_VALUE.ToString());
            RuleFor(x => x.StandardValue)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_STANDARD_VALUE.ToString());
            RuleFor(x => x.Unit)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_EXAMINATION_PARAMETER_NAME_UNIT.ToString());
        }
    }
}