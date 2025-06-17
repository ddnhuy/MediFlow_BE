namespace Appointment.API.Appointments.Commands
{
    public record CreateAppointmentResult(bool IsSuccess, string Message);
    public record CreateAppointmentCommand(int PatientId, int DepartmentId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientEmail, string? PatientPhoneNumber, string? Note) : ICommand<CreateAppointmentResult>;

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage(ValidationStrings.INVALID_PATIENT_ID);
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_ID);
            RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.UtcNow).WithMessage(ValidationStrings.INVALID_APPOINTMENT_DATE);
            RuleFor(x => x.AppointmentType).IsInEnum().WithMessage(ValidationStrings.INVALID_APPOINTMENT_TYPE);
            RuleFor(x => x.PatientEmail).NotEmpty().EmailAddress().WithMessage(ValidationStrings.INVALID_PATIENT_EMAIL);
            RuleFor(x => x.PatientPhoneNumber).Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PatientPhoneNumber)).WithMessage(ValidationStrings.INVALID_PATIENT_PHONE_NUMBER);
        }
    }

    internal class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, CreateAppointmentResult>
    {
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        public CreateAppointmentCommandHandler(ICurrentUserHelper currentUserHelper, IAppointmentRepository appointmentRepository)
        {
            _currentUserHelper = currentUserHelper;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<CreateAppointmentResult> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = new Models.Appointment
            {
                PatientId = command.PatientId,
                DepartmentId = command.DepartmentId,
                AppointmentDate = command.AppointmentDate,
                AppointmentType = command.AppointmentType,
                PatientEmail = command.PatientEmail,
                PatientPhoneNumber = command.PatientPhoneNumber,
                Note = command.Note,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserHelper.GetUserId(),
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = _currentUserHelper.GetUserId()
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return new CreateAppointmentResult(true, AppointmentSuccessStrings.AppointmentCreated);
        }
    }
}
