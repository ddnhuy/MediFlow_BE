using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Domain.Enums;

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

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

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
                .HasComment("Ngày xuất hóa đơn");

            builder.Property(x => x.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar(20)")
                .HasDefaultValue(PaymentStatusForItem.NotPaid)
                .HasComment("Trạng thái thanh toán");

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

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.RequestNumber)
                .IsRequired()
                .HasMaxLength(15)
                .HasComment("Số phiếu yêu cầu")
                .HasColumnType("varchar(15)");

            builder.Property(x => x.ReceptionId)
                .IsRequired()
                .HasComment("Mã tiếp nhận");

            // Relationships
            builder.HasOne(x => x.Reception)
                .WithMany(x => x.ServiceRequestDetails)
                .HasForeignKey(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ServiceId)
                .HasDatabaseName("IX_ServiceRequestDetails_ServiceId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng chi tiết yêu cầu dịch vụ"));
        }
    }
}
