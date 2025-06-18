using Grpc.Core;

namespace Appointment.API.Appointments.Queries
{
    public record GetAppointmentByIdResult(AppointmentDetailDto Appointment);
    public record GetAppointmentByIdQuery(int Id, string Roles) : IQuery<GetAppointmentByIdResult>;

    public class GetAppointmentByIdHandler : IQueryHandler<GetAppointmentByIdQuery, GetAppointmentByIdResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ApplicationUserProtoServiceClient _userServiceClient;
        private readonly DepartmentProtoServiceClient _departmentServiceClient;
        private readonly PatientProtoServiceClient _patientServiceClient;

        public GetAppointmentByIdHandler(
            IAppointmentRepository appointmentRepository,
            ApplicationUserProtoServiceClient userServiceClient,
            DepartmentProtoServiceClient departmentServiceClient,
            PatientProtoServiceClient patientServiceClient)
        {
            _appointmentRepository = appointmentRepository;
            _userServiceClient = userServiceClient;
            _departmentServiceClient = departmentServiceClient;
            _patientServiceClient = patientServiceClient;
        }

        public async Task<GetAppointmentByIdResult> Handle(GetAppointmentByIdQuery query, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(query.Id);

            if (appointment is null)
            {
                throw new NotFoundException(AppointmentExceptionStrings.NOT_FOUND_APPOINTMENT_WITH_ID(query.Id));
            }

            var metadata = new Metadata
            {
                { "x-roles", query.Roles }
            };

            var getCreatedBy = _userServiceClient.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = appointment.CreatedBy
            }, metadata);
            var getLastUpdatedBy = _userServiceClient.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = appointment.LastUpdatedBy
            }, metadata);
            var getPatient = _patientServiceClient.GetPatientAsync(new GetPatientRequest
            {
                Id = appointment.PatientId
            });
            var getDepartment = _departmentServiceClient.GetDepartmentAsync(new GetDepartmentRequest
            {
                Id = appointment.DepartmentId
            });

            var createdBy = await getCreatedBy;
            var lastUpdatedBy = await getLastUpdatedBy;
            var patient = await getPatient;
            var department = await getDepartment;

            return new GetAppointmentByIdResult(new AppointmentDetailDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentType = appointment.AppointmentType.ToString(),
                IsSuspended = appointment.IsSuspended,
                IsCancelled = appointment.IsCancelled,
                CreatedAt = appointment.CreatedAt,
                LastUpdatedAt = appointment.LastUpdatedAt,
                CreatedBy = new ApplicationUserDto
                {
                    Id = createdBy.Id,
                    UserName = createdBy.UserName,
                    Name = createdBy.Name,
                    Code = createdBy.Code
                },
                LastUpdatedBy = new ApplicationUserDto
                {
                    Id = lastUpdatedBy.Id,
                    UserName = lastUpdatedBy.UserName,
                    Name = lastUpdatedBy.Name,
                    Code = lastUpdatedBy.Code
                },
                Patient = new PatientDto
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    DOB = patient.Dob.ToDateTime(),
                    Email = patient.Email,
                    PhoneNumber = patient.PhoneNumber
                },
                Department = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameInEnglish = department.NameInEnglish
                }
            });
        }
    }
}