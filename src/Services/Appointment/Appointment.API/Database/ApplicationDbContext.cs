using Microsoft.EntityFrameworkCore;

namespace Appointment.API.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Models.Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Appointment>(entity =>
            {
                entity.Property(x => x.AppointmentType).HasConversion<string>();
            });

            modelBuilder.Entity<Models.Appointment>().HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
