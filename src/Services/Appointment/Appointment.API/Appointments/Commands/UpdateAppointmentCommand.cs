using FluentValidation;

namespace Appointment.API.Appointments.Commands
{
    public record UpdateAppointmentResult(bool IsSuccess, string Message);
    public record UpdateAppointmentCommand(int Id, int PatientId, int DepartmentId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientEmail, string? PatientPhoneNumber, string? Note, bool IsSuspended) : ICommand<UpdateAppointmentResult>;

    public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_APPOINTMENT_ID.ToString());
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage(ExceptionKey.INVALID_PATIENT_ID.ToString());
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_ID.ToString());
            RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.UtcNow).WithMessage(ExceptionKey.INVALID_APPOINTMENT_DATE.ToString());
            RuleFor(x => x.AppointmentType).IsInEnum().WithMessage(ExceptionKey.INVALID_APPOINTMENT_TYPE.ToString());
            RuleFor(x => x.PatientEmail).NotEmpty().EmailAddress().WithMessage(ExceptionKey.INVALID_PATIENT_EMAIL.ToString());
            RuleFor(x => x.PatientPhoneNumber).Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PatientPhoneNumber)).WithMessage(ExceptionKey.INVALID_PATIENT_PHONE_NUMBER.ToString());
            RuleFor(x => x.IsSuspended).NotNull().WithMessage(ExceptionKey.REQUIRED_SUSPENDED_STATUS.ToString());
        }
    }

    internal class UpdateAppointmentCommandHandler : ICommandHandler<UpdateAppointmentCommand, UpdateAppointmentResult>
    {
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        public UpdateAppointmentCommandHandler(ICurrentUserHelper currentUserHelper, IAppointmentRepository appointmentRepository)
        {
            _currentUserHelper = currentUserHelper;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<UpdateAppointmentResult> Handle(UpdateAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.Id);

            if (appointment is null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_APPOINTMENT_WITH_ID);
            }

            appointment.PatientId = command.PatientId;
            appointment.DepartmentId = command.DepartmentId;
            appointment.AppointmentDate = command.AppointmentDate;
            appointment.AppointmentType = command.AppointmentType;
            appointment.PatientEmail = command.PatientEmail;
            appointment.PatientPhoneNumber = command.PatientPhoneNumber;
            appointment.Note = command.Note;
            appointment.IsSuspended = command.IsSuspended;

            appointment.LastUpdatedAt = DateTime.UtcNow;
            appointment.LastUpdatedBy = _currentUserHelper.GetUserId();

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return new UpdateAppointmentResult(true, AppointmentSuccessStrings.AppointmentUpdated);
        }
    }
}
