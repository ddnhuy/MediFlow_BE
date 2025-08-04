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
    public class PaymentContractConfiguration : IEntityTypeConfiguration<PaymentContract>
    {
        public void Configure(EntityTypeBuilder<PaymentContract> builder)
        {
            builder.ToTable("PaymentContracts", schema: "public");
            builder.ToTable(t => t.HasComment("Hợp đồng thanh toán"));

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

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

            builder.Property(x => x.ContractId)
                .HasComment("Hop Dong Id");

            builder.Property(x => x.InvoiceNumber)
                .HasMaxLength(50)
                .HasComment("Số hóa đơn");

            builder.Property(x => x.VATInvoiceNumber)
                .HasMaxLength(50)
                .HasComment("Số HĐ GTGT");

            builder.Property(x => x.InvoiceType)
                .HasColumnType("integer")
                .HasComment("Loại hóa đơn");

            builder.Property(x => x.CreatedByUserId)
                .HasComment("Người lập hóa đơn");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("numeric(18,2)")
                .HasComment("Giá trị hợp đồng");

            builder.Property(x => x.PaymentMethod)
                .HasColumnType("integer")
                .HasComment("Hình thức thanh toán");

            builder.Property(x => x.Status)
                .HasColumnType("integer")
                .HasComment("Trạng thái thanh toán");

            builder.Property(x => x.TaxCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Mã số thuế đơn vị");

            builder.Property(x => x.OrganizationName)
                .IsRequired()
                .HasMaxLength(256) 
                .HasComment("Tên đơn vị thanh toán");

            builder.Property(x => x.ATMCode)
                .HasMaxLength(50)
                .HasComment("Mã giao dịch thẻ ATM");

            builder.HasOne(x => x.Contract)
                .WithMany(c => c.PaymentContracts)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global Query Filter (inherited from BaseEntity logic)
            builder.HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
