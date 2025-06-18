using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstractions.CurrentUser;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data.Extensions;
using VaccinationReception.Infrastructure.Helpers;

namespace VaccinationReception.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserHelper _userHelper;
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserHelper userHelper,
            ILogger<ApplicationDbContext> logger) : base(options)
        {
            _userHelper = userHelper;
            _logger = logger;
        }

        public virtual DbSet<Reception> Receptions { get; set; }
        public virtual DbSet<ScreeningEvaluationReport> ScreeningEvaluationReports { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ReceptionVaccination> ReceptionVaccinations { get; set; }
        public virtual DbSet<DiseaseGroup> DiseaseGroups { get; set; }
        public virtual DbSet<DiseaseGroupService> DiseaseGroupServices { get; set; }
        public virtual DbSet<ServiceGroup> ServiceGroups { get; set; }
        public virtual DbSet<ServiceGroupService> ServiceGroupServices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceRequestDetail> ServiceRequestDetails { get; set; }
        public virtual DbSet<RequestForm> RequestForms { get; set; }
        public virtual DbSet<Vaccination> Vaccinations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.SeedData();
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                SetUpdatedAt();
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error saving changes to database");
                throw;
            }
        }

        private void SetUpdatedAt()
        {
            var userId = _userHelper.UserId;
            _logger.LogDebug("Current user ID: {UserId}", userId);

            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IEntity)entry.Entity;
                entity.LastUpdatedAt = DateTime.UtcNow;
                entity.LastUpdatedBy = userId == 0 ? 1 : userId;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedBy = userId == 0 ? 1 : userId;
                }

                _logger.LogDebug( "Updated entity {EntityType} (ID: {EntityId}) - State: {State}, CreatedBy: {CreatedBy}, LastUpdatedBy: {LastUpdatedBy}",
                    entry.Entity.GetType().Name,
                    entry.Property("Id").CurrentValue,
                    entry.State,
                    entity.CreatedBy,
                    entity.LastUpdatedBy);
            }
        }
    }
}