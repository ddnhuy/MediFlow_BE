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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
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
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired();

            // Fields
            builder.Property(x => x.ReceptionId)
                .IsRequired()
                .HasComment("Mã tiếp nhận");

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasColumnType("numeric(18,2)")
                .HasComment("Tổng số tiền");

            builder.Property(x => x.Method)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar(20)")
                .HasComment("Phương thức thanh toán");

            builder.Property(x => x.Note)
                .HasComment("Ghi chú");

            builder.Property(x => x.ATMTransactionCode)
                .HasComment("Mã giao dịch ATM");

            builder.Property(x => x.PaymentType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired()
                .HasComment("Loại thanh toán");

            builder.Property(x => x.InvoiceNumber)
                .HasComment("Số hóa đơn tạm");

            builder.Property(x => x.OfficialInvoiceNumber)
                .HasComment("Số hóa đơn chính thức");

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasComment("Trạng thái thanh toán");

            // Relationships
            builder.HasOne(x => x.Reception)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OriginalPayment)
                .WithMany()
                .HasForeignKey(x => x.OriginalPaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PaymentDetails)
                .WithOne(x => x.Payment)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ReceptionId)
                .HasDatabaseName("IX_Payments_ReceptionId");

            builder.HasQueryFilter(x => !x.IsCancelled);

            builder.ToTable(t => t.HasComment("Bảng thanh toán"));
        }
    }
}
