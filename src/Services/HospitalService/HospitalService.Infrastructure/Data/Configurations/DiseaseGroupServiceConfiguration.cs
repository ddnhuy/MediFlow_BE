using HospitalService.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Data.Configurations
{
    public class DiseaseGroupServiceConfiguration : IEntityTypeConfiguration<DiseaseGroupService>
    {
        public void Configure(EntityTypeBuilder<DiseaseGroupService> builder)
        {
            builder.ToTable("DiseaseGroupServices", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity Properties
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái hủy")
                .HasColumnType("boolean");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.IsSuspended)
               .IsRequired()
               .HasDefaultValue(false)
               .HasComment("Trạng thái tạm ngưng")
               .HasColumnType("boolean");

            // Properties
            builder.Property(x => x.DiseaseGroupId)
                .IsRequired()
                .HasComment("Mã nhóm bệnh");

            builder.Property(x => x.ServiceId)
                .IsRequired()
                .HasComment("Mã dịch vụ");

            // Relationships
            builder.HasOne(x => x.DiseaseGroup)
                .WithMany(x => x.DiseaseGroupServices)
                .HasForeignKey(x => x.DiseaseGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Service)
                .WithMany(x => x.DiseaseGroupServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.DiseaseGroupId)
                .HasDatabaseName("IX_DiseaseGroupServices_DiseaseGroupId");

            builder.HasIndex(x => x.ServiceId)
                .HasDatabaseName("IX_DiseaseGroupServices_ServiceId");

            builder.HasIndex(x => new { x.DiseaseGroupId, x.ServiceId })
                .IsUnique()
                .HasDatabaseName("IX_DiseaseGroupServices_DiseaseGroupId_ServiceId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng liên kết nhóm bệnh và dịch vụ"));
        }
    }
}
