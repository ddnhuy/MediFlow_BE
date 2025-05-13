using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace HumanResource.Grpc.Database
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserHelper userHelper)
        : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
    {
        public virtual DbSet<DepartmentType> DepartmentTypes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);

            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
            {
                string? tableName = entityType.GetTableName();
                if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith("AspNet", StringComparison.CurrentCulture))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUpdatedAt();

            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private void SetUpdatedAt()
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var userId = userHelper.UserId;

            foreach (EntityEntry entry in entries)
            {
                var entity = (IEntity)entry.Entity;
                entity.LastUpdatedAt = DateTime.UtcNow;
                entity.LastUpdatedBy = userId == 0 ? 1 : userId;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedBy = userId == 0 ? 1 : userId;
                }
            }
        }
    }
}
