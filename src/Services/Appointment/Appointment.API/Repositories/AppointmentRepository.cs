using Appointment.API.Database;
using Microsoft.EntityFrameworkCore;

namespace Appointment.API.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Models.Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Models.Appointment>> GetAllAsync();
        Task<(IEnumerable<Models.Appointment>, int totalCount)> GetUpcomingAppointmentsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? doctorId = null,
            TimeOfDayFilter? timeOfDay = null,
            int? vaccineId = null,
            int pageIndex = 1,
            int pageSize = 10
        );
        Task<IEnumerable<Models.Appointment>> GetAppointmentsByPatientIdAsync(int patientId);

        Task AddAsync(Models.Appointment appointment);
        Task UpdateAsync(Models.Appointment appointment);
        Task DeleteAsync(Models.Appointment appointment);

        Task SaveChangesAsync();
    }

    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Models.Appointment> _appointments;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
            _appointments = context.Set<Models.Appointment>();
        }

        public async Task<Models.Appointment?> GetByIdAsync(int id)
        {
            return await _appointments.FindAsync(id);
        }

        public async Task<IEnumerable<Models.Appointment>> GetAllAsync()
        {
            return await _appointments.AsNoTracking().ToListAsync();
        }

        public async Task<(IEnumerable<Models.Appointment>, int totalCount)> GetUpcomingAppointmentsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? doctorId = null,
            TimeOfDayFilter? timeOfDay = null,
            int? vaccineId = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _appointments.AsQueryable();

            if (fromDate.HasValue)
            {
                // Convert to UTC and get the date part only
                var utcFromDate = fromDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc).Date
                    : fromDate.Value.ToUniversalTime().Date;
                query = query.Where(a => a.AppointmentDate.Date >= utcFromDate);
            }

            if (toDate.HasValue)
            {
                // Convert to UTC and get the date part only
                var utcToDate = toDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc).Date
                    : toDate.Value.ToUniversalTime().Date;
                query = query.Where(a => a.AppointmentDate.Date <= utcToDate);
            }

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (vaccineId.HasValue)
                query = query.Where(a => a.VaccineId == vaccineId.Value);

            if (timeOfDay.HasValue)
            {
                if (timeOfDay == TimeOfDayFilter.Morning)
                {
                    var morningStart = TimeSpan.Zero;
                    var morningEnd = new TimeSpan(12, 0, 0);
                    query = query.Where(a => a.AppointmentDate.TimeOfDay >= morningStart
                                         && a.AppointmentDate.TimeOfDay < morningEnd);
                }
                else if (timeOfDay == TimeOfDayFilter.Afternoon)
                {
                    var afternoonStart = new TimeSpan(12, 0, 0);
                    var afternoonEnd = new TimeSpan(24, 0, 0);
                    query = query.Where(a => a.AppointmentDate.TimeOfDay >= afternoonStart
                                         && a.AppointmentDate.TimeOfDay < afternoonEnd);
                }
            }

            query = query.Where(a => !a.IsSuspended && !a.IsCancelled);

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<IEnumerable<Models.Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _appointments
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task AddAsync(Models.Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.LastUpdatedAt = DateTime.UtcNow;
            await _appointments.AddAsync(appointment);
        }

        public Task UpdateAsync(Models.Appointment appointment)
        {
            appointment.LastUpdatedAt = DateTime.UtcNow;
            _appointments.Update(appointment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Models.Appointment appointment)
        {
            appointment.IsSuspended = true;
            appointment.IsCancelled = true;
            _appointments.Update(appointment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
