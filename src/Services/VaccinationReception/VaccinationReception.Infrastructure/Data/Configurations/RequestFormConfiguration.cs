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
    public class RequestFormConfiguration : IEntityTypeConfiguration<RequestForm>
    {
        public void Configure(EntityTypeBuilder<RequestForm> builder)
        {
            builder.ToTable("RequestForms", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

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

            builder.Property(x => x.RequestNumber)
                .IsRequired()
                .HasMaxLength(15)
                .HasComment("Số phiếu yêu cầu")
                .HasColumnType("varchar(15)");

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.ReceptionId)
                .IsRequired()
                .HasComment("Mã tiếp nhận");

            builder.Property(x => x.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .HasComment("Ngày tạo phiếu");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo phiếu");

            builder.HasOne(x => x.Reception)
                .WithMany(x => x.RequestForms)
                .HasForeignKey(x => x.ReceptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ServiceRequestDetails)
                .WithOne(x => x.RequestForm)
                .HasForeignKey(x => x.RequestFormId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.RequestNumber)
                .IsUnique()
                .HasDatabaseName("IX_RequestForms_RequestNumber");

            builder.HasIndex(x => x.ReceptionId)
                .HasDatabaseName("IX_RequestForms_ReceptionId");

            builder.HasQueryFilter(x => !x.IsCancelled);

            builder.ToTable(t => t.HasComment("Bảng phiếu yêu cầu dịch vụ"));
        }
    }
}
