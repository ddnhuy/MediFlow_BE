using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Helpers;

namespace VaccinationReception.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
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
        public virtual DbSet<ScreeningEvaluation> ScreeningEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                SetUpdatedAt();
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }

        private void SetUpdatedAt()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            var userId = _userHelper.UserId;
            _logger.LogDebug("Current user ID: {UserId}", userId);

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

                _logger.LogDebug(
                    "Updated entity {EntityType} (ID: {EntityId}) - State: {State}, CreatedBy: {CreatedBy}, LastUpdatedBy: {LastUpdatedBy}",
                    entry.Entity.GetType().Name,
                    entry.Property("Id").CurrentValue,
                    entry.State,
                    entity.CreatedBy,
                    entity.LastUpdatedBy);
            }
        }
    }
}