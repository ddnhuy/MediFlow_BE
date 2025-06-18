namespace Appointment.API.Appointments.Commands
{
    public record DeleteAppointmentResult(bool IsSuccess, string Message);
    public record DeleteAppointmentCommand(int Id) : ICommand<DeleteAppointmentResult>;

    public class DeleteAppointmentCommandValidator : AbstractValidator<DeleteAppointmentCommand>
    {
        public DeleteAppointmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_APPOINTMENT_ID);
        }
    }

    internal class DeleteAppointmentCommandHandler : ICommandHandler<DeleteAppointmentCommand, DeleteAppointmentResult>
    {
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        public DeleteAppointmentCommandHandler(ICurrentUserHelper currentUserHelper, IAppointmentRepository appointmentRepository)
        {
            _currentUserHelper = currentUserHelper;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<DeleteAppointmentResult> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.Id);

            if (appointment is null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_APPOINTMENT_WITH_ID);
            }

            appointment.LastUpdatedAt = DateTime.UtcNow;
            appointment.LastUpdatedBy = _currentUserHelper.GetUserId();

            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return new DeleteAppointmentResult(true, AppointmentSuccessStrings.AppointmentDeleted);
        }
    }
}
