namespace Appointment.API.Appointments.Commands
{
    public record CreateAppointmentResult(bool IsSuccess, string Message);
    public record CreateAppointmentCommand(int UserId, int PatientId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientCode, string PatientFullName, DateTime PatientDOB, string PatientEmail, string? PatientPhoneNumber, string? VaccineName, string? Note, int DoctorId, int VaccineId, string? Dose) : ICommand<CreateAppointmentResult>;

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage(ExceptionKey.INVALID_PATIENT_ID.ToString());
            RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.UtcNow).WithMessage(ExceptionKey.INVALID_APPOINTMENT_DATE.ToString());
            RuleFor(x => x.AppointmentType).IsInEnum().WithMessage(ExceptionKey.INVALID_APPOINTMENT_TYPE.ToString());
            RuleFor(x => x.PatientCode).NotEmpty().WithMessage(ExceptionKey.REQUIRED_PATIENT_CODE.ToString());
            RuleFor(x => x.PatientFullName).NotEmpty().WithMessage(ExceptionKey.REQUIRED_PATIENT_NAME.ToString());
            RuleFor(x => x.PatientDOB).LessThan(DateTime.UtcNow).WithMessage(ExceptionKey.INVALID_PATIENT_DOB.ToString());
            RuleFor(x => x.PatientEmail).NotEmpty().EmailAddress().WithMessage(ExceptionKey.INVALID_PATIENT_EMAIL.ToString());
            RuleFor(x => x.PatientPhoneNumber).Matches(@"^\+?[0-9]*$").When(x => !string.IsNullOrEmpty(x.PatientPhoneNumber)).WithMessage(ExceptionKey.INVALID_PATIENT_PHONE_NUMBER.ToString());
            RuleFor(x => x.DoctorId).GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_DOCTOR_ID.ToString());
            RuleFor(x => x.VaccineId).GreaterThan(0).WithMessage(ExceptionKey.INVALID_VACCINE_ID.ToString());
            RuleFor(x => x.VaccineName).NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_NAME.ToString());
        }
    }

    internal class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, CreateAppointmentResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<CreateAppointmentResult> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = new Models.Appointment
            {
                PatientId = command.PatientId,
                AppointmentDate = command.AppointmentDate,
                AppointmentType = command.AppointmentType,
                PatientCode = command.PatientCode,
                PatientFullName = command.PatientFullName,
                PatientDOB = command.PatientDOB,
                PatientEmail = command.PatientEmail,
                PatientPhoneNumber = command.PatientPhoneNumber,
                VaccineName = command.VaccineName,
                Note = command.Note,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.UserId,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = command.UserId,
                DoctorId = command.DoctorId,
                VaccineId = command.VaccineId,
                Dose = command.Dose ?? string.Empty
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return new CreateAppointmentResult(true, AppointmentSuccessStrings.AppointmentCreated);
        }
    }
}