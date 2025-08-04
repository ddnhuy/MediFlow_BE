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
    public class ContractServiceDetailConfiguration : IEntityTypeConfiguration<ContractServiceDetail>
    {
        public void Configure(EntityTypeBuilder<ContractServiceDetail> builder)
        {
            // Table and Schema
            builder.ToTable("ContractServiceDetails", schema: "public");
            builder.ToTable(t => t.HasComment("Chi tiết dịch vụ/vắc-xin trong hợp đồng"));

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
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.ContractId)
                .IsRequired()
                .HasComment("Mã hợp đồng");

            builder.Property(x => x.VaccineId)
                .IsRequired(false)
                .HasComment("Mã vắc-xin");

            builder.Property(x => x.ServiceId)
                .IsRequired(false)
                .HasComment("Mã dịch vụ");

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasComment("Số lượng dự kiến theo hợp đồng");

            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasColumnType("numeric(18,2)")
                .HasComment("Đơn giá của dịch vụ/vắc-xin này theo hợp đồng");

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasColumnType("numeric(18,2)")
                .HasComment("Tổng tiền cho mục này");

            builder.Property(x => x.Quantity)
                .HasComment("Số lượng thực tế");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("numeric(18,2)")
                .HasComment("Tổng tiền thực tế cho mục này");

            builder.HasOne(x => x.Contract)
                .WithMany(c => c.ServiceDetails)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
