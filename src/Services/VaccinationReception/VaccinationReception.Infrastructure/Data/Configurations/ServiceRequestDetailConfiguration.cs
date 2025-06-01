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
    public class ServiceRequestDetailConfiguration : IEntityTypeConfiguration<ServiceRequestDetail>
    {
        public void Configure(EntityTypeBuilder<ServiceRequestDetail> builder)
        {
            builder.ToTable("ServiceRequestDetails", schema: "public");

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            // Properties
            builder.Property(x => x.RequestFormId)
                .IsRequired()
                .HasComment("Mã phiếu yêu cầu");

            builder.Property(x => x.ServiceId)
                .IsRequired()
                .HasComment("Mã dịch vụ");

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasComment("Số lượng");

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Đơn giá");

            builder.Property(x => x.InvoiceDate)
                .IsRequired()
                .HasComment("Ngày xuất hóa đơn")
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.IsPaid)
                .IsRequired()
                .HasComment("Đã thanh toán")
                .HasColumnType("boolean");

            // Relationships
            builder.HasOne(x => x.RequestForm)
                .WithMany(x => x.ServiceRequestDetails)
                .HasForeignKey(x => x.RequestFormId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Service)
                .WithMany(x => x.ServiceRequestDetails)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.RequestFormId)
                .HasDatabaseName("IX_ServiceRequestDetails_RequestFormId");

            builder.HasIndex(x => x.ServiceId)
                .HasDatabaseName("IX_ServiceRequestDetails_ServiceId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng chi tiết yêu cầu dịch vụ"));
        }
    }
}
