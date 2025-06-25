using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Infrastructure.Data.Configurations
{
    public class ReceptionVaccinationConfiguration : IEntityTypeConfiguration<ReceptionVaccination>
    {
        public void Configure(EntityTypeBuilder<ReceptionVaccination> builder)
        {
            builder.ToTable("ReceptionVaccinations", schema: "public");

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

            // Properties
            builder.Property(x => x.RequestNumber)
                .IsRequired()
                .HasMaxLength(15)
                .HasComment("Số phiếu yêu cầu")
                .HasColumnType("varchar(15)");

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Đơn giá");

            builder.Property(x => x.ReceptionId)
                .IsRequired()
                .HasComment("Mã tiếp nhận");

            builder.Property(x => x.VaccineId)
                .IsRequired()
                .HasComment("Mã vắc xin");

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasComment("Số lượng");

            builder.Property(x => x.IsReadyToUse)
                .IsRequired()
                .HasComment("Sẵn sàng sử dụng")
                .HasColumnType("boolean");

            builder.Property(x => x.ScheduledDate)
                .HasComment("Ngày dự kiến tiêm");

            builder.Property(x => x.InvoiceDate)
                .HasComment("Ngày xuất hóa đơn");

            builder.Property(x => x.AppointmentDate)
                .HasComment("Ngày hẹn tiêm");

            builder.Property(x => x.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar(20)")
                .HasDefaultValue(PaymentStatusForItem.NotPaid)
                .HasComment("Trạng thái thanh toán");

            builder.Property(x => x.IsConfirmed)
                .IsRequired()
                .HasComment("Đã xác nhận")
                .HasColumnType("boolean");

            builder.Property(x => x.Note)
                .HasMaxLength(255)
                .HasComment("Ghi chú");

            builder.Property(x => x.TestResultEntry)
                .HasMaxLength(255)
                .HasComment("Kết quả thử");

            builder.Property(x => x.DoctorId)
                .IsRequired()
                .HasComment("Mã bác sĩ");

            // Relationships
            builder.HasOne(x => x.Reception)
                .WithMany(r => r.ReceptionVaccinations)
                .HasForeignKey(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.ReceptionId)
                .HasDatabaseName("IX_ReceptionVaccinations_ReceptionId");

            builder.HasIndex(x => x.VaccineId)
                .HasDatabaseName("IX_ReceptionVaccinations_VaccineId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng chỉ định tiêm chủng"));
        }
    }
}