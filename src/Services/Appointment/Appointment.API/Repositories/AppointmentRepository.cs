using Appointment.API.Database;
using Microsoft.EntityFrameworkCore;

namespace Appointment.API.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Models.Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Models.Appointment>> GetAllAsync();
        Task<IEnumerable<Models.Appointment>> GetUpcomingAppointmentsAsync(DateTime fromDate);
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

        public async Task<IEnumerable<Models.Appointment>> GetUpcomingAppointmentsAsync(DateTime fromDate)
        {
            return await _appointments
                .Where(a => a.AppointmentDate >= fromDate && !a.IsSuspended && !a.IsCancelled)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
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
