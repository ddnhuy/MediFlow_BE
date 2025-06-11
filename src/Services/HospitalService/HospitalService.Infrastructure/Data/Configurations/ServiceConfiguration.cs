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
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services", schema: "public");

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

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái hủy")
                .HasColumnType("boolean");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            // Properties
            builder.Property(x => x.ServiceCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Mã dịch vụ");

            builder.Property(x => x.ServiceName)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Tên dịch vụ");

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Đơn giá");

            builder.Property(x => x.DepartmentId)
                .IsRequired()
                .HasComment("Mã phòng ban");

            // Relationships
            builder.HasMany(x => x.ServiceGroupServices)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DiseaseGroupServices)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ServiceCode)
                .IsUnique()
                .HasDatabaseName("IX_Services_ServiceCode");

            builder.HasIndex(x => x.ServiceName)
                .HasDatabaseName("IX_Services_ServiceName");

            builder.HasIndex(x => x.DepartmentId)
               .HasDatabaseName("IX_Services_DepartmentId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng dịch vụ"));
        }
    }
}
