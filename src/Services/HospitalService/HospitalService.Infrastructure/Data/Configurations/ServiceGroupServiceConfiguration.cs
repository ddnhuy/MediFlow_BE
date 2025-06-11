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
    public class ServiceGroupServiceConfiguration : IEntityTypeConfiguration<ServiceGroupService>
    {
        public void Configure(EntityTypeBuilder<ServiceGroupService> builder)
        {
            builder.ToTable("ServiceGroupServices", schema: "public");

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

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            // Properties
            builder.Property(x => x.ServiceGroupId)
                .IsRequired()
                .HasComment("Mã nhóm dịch vụ");

            builder.Property(x => x.ServiceId)
                .IsRequired()
                .HasComment("Mã dịch vụ");

            // Relationships
            builder.HasOne(x => x.ServiceGroup)
                .WithMany(x => x.ServiceGroupServices)
                .HasForeignKey(x => x.ServiceGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Service)
                .WithMany(x => x.ServiceGroupServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ServiceGroupId)
                .HasDatabaseName("IX_ServiceGroupServices_ServiceGroupId");

            builder.HasIndex(x => x.ServiceId)
                .HasDatabaseName("IX_ServiceGroupServices_ServiceId");

            builder.HasIndex(x => new { x.ServiceGroupId, x.ServiceId })
                .IsUnique()
                .HasDatabaseName("IX_ServiceGroupServices_ServiceGroupId_ServiceId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng liên kết nhóm dịch vụ và dịch vụ"));
        }
    }
}
