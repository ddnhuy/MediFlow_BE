using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Infrastructure.Data.Configurations
{
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("ServiceTypes", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity
            builder.Property(x => x.IsSuspended)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái tạm ngưng");

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Trạng thái hủy");

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

            // Fields
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Mã loại dịch vụ");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Tên loại dịch vụ");

            builder.HasMany(x => x.Receptions)
                .WithOne(x => x.ServiceType)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("IX_ServiceTypes_Code");

            // Global filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table comment
            builder.ToTable(t => t.HasComment("Loại hình dịch vụ tiếp nhận"));
        }
    }
}