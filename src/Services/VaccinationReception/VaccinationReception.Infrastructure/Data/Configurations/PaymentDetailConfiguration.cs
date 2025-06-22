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
    public class PaymentDetailConfiguration : IEntityTypeConfiguration<PaymentDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentDetail> builder)
        {
            builder.ToTable("PaymentDetails", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity properties
            builder.Property(x => x.IsSuspended)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired();

            // Fields
            builder.Property(x => x.PaymentId)
                .IsRequired()
                .HasComment("Mã thanh toán");

            builder.Property(x => x.ReceptionVaccinationId)
                .HasComment("Mã tiêm chủng");

            builder.Property(x => x.ServiceRequestDetailId)
                .HasComment("Mã chi tiết yêu cầu dịch vụ");

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("numeric(18,2)")
                .HasComment("Số tiền");

            // Relationships
            builder.HasOne(x => x.Payment)
                .WithMany(x => x.PaymentDetails)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReceptionVaccination)
                .WithMany()
                .HasForeignKey(x => x.ReceptionVaccinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceRequestDetail)
                .WithMany()
                .HasForeignKey(x => x.ServiceRequestDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.PaymentId)
                .HasDatabaseName("IX_PaymentDetails_PaymentId");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            builder.ToTable(t => t.HasComment("Chi tiết thanh toán"));
        }
    }
}
