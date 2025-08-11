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
    public class ScreeningEvaluationReportConfiguration : IEntityTypeConfiguration<ScreeningEvaluationReport>
    {
        public void Configure(EntityTypeBuilder<ScreeningEvaluationReport> builder)
        {
            // Table
            builder.ToTable("ScreeningEvaluationReports", schema: "public");
            builder.ToTable(t => t.HasComment("Bảng ghi nhận đánh giá sàng lọc trước tiêm"));

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .UseIdentityColumn()
                   .ValueGeneratedOnAdd()
                   .HasComment("Khóa chính")
                   .HasAnnotation("Npgsql:IdentityIncrement", 1)
                   .HasAnnotation("Npgsql:IdentityStartValue", 1);

            builder.Property(x => x.LastUpdatedAt)
                   .IsRequired()
                   .HasComment("Ngày cập nhật");

            builder.Property(x => x.LastUpdatedBy)
                   .IsRequired()
                   .HasComment("Người cập nhật");

            // Parent Information
            builder.Property(x => x.ParentFullName)
                   .HasMaxLength(100)
                   .HasColumnType("character varying(100)")
                   .HasComment("Họ tên phụ huynh");

            builder.Property(x => x.ParentPhoneNumber)
                   .HasMaxLength(20)
                   .HasColumnType("character varying(20)")
                   .HasComment("Số điện thoại phụ huynh");

            builder.Property(x => x.CreatedAt)
                   .IsRequired()
                   .HasComment("Ngày tạo");

            builder.Property(x => x.CreatedBy)
                   .IsRequired()
                   .HasComment("Người tạo");

            // Screening info
            builder.Property(x => x.WeightKg)
                   .HasComment("Cân nặng (kg)");

            builder.Property(x => x.BodyTemperatureC)
                   .HasComment("Nhiệt độ cơ thể (°C)");

            builder.Property(x => x.BloodPressureSystolic)
                   .HasComment("Huyết áp tâm thu");

            builder.Property(x => x.BloodPressureDiastolic)
                   .HasComment("Huyết áp tâm trương");

            // Boolean screening items
            builder.Property(x => x.HasSevereFeverAfterPreviousVaccination).HasComment("Sốt nặng sau tiêm trước");
            builder.Property(x => x.HasAcuteOrChronicDisease).HasComment("Bệnh cấp/mạn tính");
            builder.Property(x => x.IsOnOrRecentlyEndedCorticosteroids).HasComment("Đang/đã dùng corticosteroid");
            builder.Property(x => x.HasAbnormalTemperatureOrVitals).HasComment("Nhiệt độ/sinh hiệu bất thường");
            builder.Property(x => x.HasAbnormalHeartSound).HasComment("Nghe tim bất thường");
            builder.Property(x => x.HasHeartValveDisorder).HasComment("Rối loạn van tim");
            builder.Property(x => x.HasNeurologicalAbnormalities).HasComment("Bất thường thần kinh");
            builder.Property(x => x.IsUnderweightBelow2000g).HasComment("Thiếu cân < 2000g");
            builder.Property(x => x.HasOtherContraindications).HasComment("Chống chỉ định khác");

            builder.Property(x => x.HasAbnormalCry).HasComment("Khóc bất thường");
            builder.Property(x => x.HasPaleSkinOrLips).HasComment("Da hoặc môi nhợt nhạt");
            builder.Property(x => x.HasPoorFeeding).HasComment("Bú kém");
            builder.Property(x => x.IsPretermBelow34Weeks).HasComment("Sinh non < 34 tuần");
            builder.Property(x => x.HasImmunodeficiencyOrSuspectedHiv).HasComment("Suy giảm miễn dịch hoặc nghi ngờ HIV");

            builder.Property(x => x.IsEligibleForVaccination).HasComment("Đủ điều kiện tiêm");
            builder.Property(x => x.IsContraindicatedForVaccination).HasComment("Chống chỉ định");
            builder.Property(x => x.IsVaccinationDeferred).HasComment("Tạm hoãn");
            builder.Property(x => x.IsReferredToHospital).HasComment("Chuyển viện");


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

            builder.HasOne(x => x.Reception)
                   .WithOne(x => x.ScreeningEvaluationReport)
                   .HasForeignKey<ScreeningEvaluationReport>(x => x.ReceptionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ReceptionId)
                   .IsRequired()
                   .HasComment("Khóa ngoại đến bảng tiếp nhận");

            // Indexes
            builder.HasIndex(x => x.ReceptionId)
                   .HasDatabaseName("IX_ScreeningEvaluationReports_ReceptionId");

            // Global query filter
            builder.HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
