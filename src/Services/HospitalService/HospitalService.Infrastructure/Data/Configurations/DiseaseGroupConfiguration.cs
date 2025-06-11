using HospitalService.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Data.Configurations
{
    public class DiseaseGroupConfiguration : IEntityTypeConfiguration<DiseaseGroup>
    {
        public void Configure(EntityTypeBuilder<DiseaseGroup> builder)
        {
            builder.ToTable("DiseaseGroups", schema: "public");

            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key")
                .HasAnnotation("Npgsql:IdentityIncrement", 1)
                .HasAnnotation("Npgsql:IdentityStartValue", 1);

            // BaseEntity Properties

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

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

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

            // Properties
            builder.Property(x => x.GroupName)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Tên nhóm bệnh");

            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .HasComment("Mô tả nhóm bệnh");

            // Relationships
            builder.HasMany(x => x.DiseaseGroupServices)
                .WithOne(x => x.DiseaseGroup)
                .HasForeignKey(x => x.DiseaseGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.GroupName)
                .HasDatabaseName("IX_DiseaseGroups_GroupName");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng nhóm bệnh"));
        }
    }

}
