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
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            // Table and Schema
            builder.ToTable("Contracts", schema: "public");
            builder.ToTable(t => t.HasComment("Hợp đồng")); // Table comment

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity properties (assuming BaseEntity includes these)
            builder.Property(x => x.IsSuspended)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.ContractCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Mã hợp đồng");

            builder.Property(x => x.ContractNumber)
                .IsRequired()
                .HasComment("Số hợp đồng");

            builder.Property(x => x.ContractName)
                .IsRequired()
                .HasMaxLength(256)
                .HasComment("Tên hợp đồng");

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(256)
                .HasComment("Tên công ty ký kết");

            builder.Property(x => x.UnitName)
                .IsRequired()
                .HasMaxLength(256)
                .HasComment("Tên đơn vị trực thuộc công ty");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("integer")
                .HasComment("Trạng thái hợp đồng");

            builder.Property(x => x.ExpectedPatientCount)
                .IsRequired()
                .HasComment("Số lượng bệnh nhân dự kiến");

            builder.Property(x => x.ExpectedVaccineCount)
                .IsRequired()
                .HasComment("Số lượng vaccine dự kiến");

            builder.Property(x => x.ContractDate)
                .IsRequired()
                .HasComment("Ngày ký hợp đồng");

            builder.Property(x => x.ExpectedDate)
                .IsRequired()
                .HasComment("Ngày dự kiến tiêm theo kế hoạch");

            builder.Property(x => x.ContractValue)
                .IsRequired()
                .HasColumnType("numeric(18,2)")
                .HasComment("Giá trị hợp đồng");

            builder.Property(x => x.AdvanceAmount)
                .HasColumnType("numeric(18,2)")
                .HasComment("Giá trị tạm ứng");

            builder.Property(x => x.ActualAmount)
                .HasColumnType("numeric(18,2)")
                .HasComment("Giá trị thực tế");

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .HasComment("Diễn giải nội dung");

            builder.Property(x => x.FileContractId)
                .HasComment("File hợp đồng id");

            builder.Property(x => x.FileVaccinationEnrollmentId)
                .HasComment("File excel đăng ký vacicnation id");

            builder.Property(x => x.FileContractName)
                .HasComment("File hợp đồng");

            builder.Property(x => x.FileVaccinationEnrollmentName)
                .HasComment("File excel đăng ký vacicnation");

            builder.HasMany(c => c.Receptions)
                .WithOne()
                .HasForeignKey("ContractId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ServiceDetails)
                .WithOne()
                .HasForeignKey("ContractId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.PlannedPatientVaccinations)
                .WithOne()
                .HasForeignKey("ContractId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.PaymentContracts)
                .WithOne(pc => pc.Contract)
                .HasForeignKey(pc => pc.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ContractCode)
                .IsUnique()
                .HasFilter("\"IsSuspended\" = false AND \"IsCancelled\" = false")
                .HasDatabaseName("IX_Contracts_ContractCode_Active");

            builder.HasIndex(x => x.ContractNumber)
                .IsUnique()
                .HasFilter("\"IsSuspended\" = false AND \"IsCancelled\" = false")
                .HasDatabaseName("IX_Contracts_ContractNumber_Active");

            // Global Query Filter (inherited from BaseEntity logic)
            builder.HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
