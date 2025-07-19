using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Infrastructure.Data.Configurations
{
    public class ReceptionConfiguration : IEntityTypeConfiguration<Reception>
    {
        public void Configure(EntityTypeBuilder<Reception> builder)
        {
            builder.ToTable("Receptions", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity Properties
            builder.Property(x => x.IsSuspended)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái tạm ngưng")
                .HasColumnType("boolean");

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái hủy")
                .HasColumnType("boolean");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            // PatientId
            builder.Property(x => x.PatientId)
                .IsRequired()
                .HasComment("Mã bệnh nhân")
                .HasColumnType("integer");

            // ReceptionDate
            builder.Property(x => x.ReceptionDate)
                .IsRequired()
                .HasComment("Ngày tiếp nhận");

            // ServiceTypeId
            builder.Property(x => x.ServiceTypeId)
                .IsRequired()
                .HasComment("Loại dịch vụ");

            // Relationships
            builder.HasOne(x => x.ScreeningEvaluationReport)
                .WithOne(x => x.Reception)
                .HasForeignKey<ScreeningEvaluationReport>(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ServiceType)
                .WithMany(x => x.Receptions)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ServiceRequestDetails)
               .WithOne(x => x.Reception)
               .HasForeignKey(x => x.ReceptionId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ReceptionVaccinations)
               .WithOne(x => x.Reception)
               .HasForeignKey(x => x.ReceptionId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Reception)
                .HasForeignKey(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.PatientId)
                .HasDatabaseName("IX_Receptions_PatientId");

            builder.HasIndex(x => x.ReceptionDate)
                .HasDatabaseName("IX_Receptions_ReceptionDate");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng tiếp nhận bệnh nhân"));
        }
    }
}