using FileStorage.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.API.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<FileMetadata> FileMetadatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileMetadata>(entity =>
            {
                entity.Property(x => x.Type).HasConversion<string>();
            });
        }
    }
}
