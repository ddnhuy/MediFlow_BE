using CustomerInfo.Grpc.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.Grpc.Database.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients", schema: "public");

            // Primary Key Configuration
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasComment("Primary key");

            // Base Entity Properties

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasComment("Ngày tạo bản ghi");

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasComment("Người cập nhật bản ghi cuối cùng");

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasComment("Người tạo bản ghi");

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

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired()
                .HasComment("Ngày cập nhật bản ghi cuối cùng");

            // Code
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Mã bệnh nhân")
                .HasColumnType("character varying(50)");

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Tên bệnh nhân")
                .HasColumnType("character varying(100)");

            // Gender
            builder.Property(x => x.Gender)
                .IsRequired()
                .HasComment("Giới tính (0: Nữ, 1: Nam)")
                .HasColumnType("smallint");

            // DOB
            builder.Property(x => x.DOB)
                .IsRequired()
                .HasComment("Ngày sinh");

            // PhoneNumber
            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .HasComment("Số điện thoại")
                .HasColumnType("character varying(20)");

            // IdentityCard
            builder.Property(x => x.IdentityCard)
                .HasMaxLength(50)
                .HasComment("CMND/CCCD")
                .HasColumnType("character varying(50)");

            // AddressDetail
            builder.Property(x => x.AddressDetail)
                .HasMaxLength(200)
                .HasComment("Địa chỉ chi tiết")
                .HasColumnType("character varying(200)");

            // Province
            builder.Property(x => x.Province)
                .HasMaxLength(100)
                .HasComment("Tỉnh/Thành phố")
                .HasColumnType("character varying(100)");

            // District
            builder.Property(x => x.District)
                .HasMaxLength(100)
                .HasComment("Quận/Huyện")
                .HasColumnType("character varying(100)");

            // Ward
            builder.Property(x => x.Ward)
                .HasMaxLength(100)
                .HasComment("Phường/Xã")
                .HasColumnType("character varying(100)");

            // IsPregnant
            builder.Property(x => x.IsPregnant)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Có thai hay không")
                .HasColumnType("boolean");

            // IsForeigner
            builder.Property(x => x.IsForeigner)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Có phải người nước ngoài hay không")
                .HasColumnType("boolean");

            // Indexes
            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("\"IsSuspended\" = false AND \"IsCancelled\" = false")
                .HasDatabaseName("IX_Patients_Code");

            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Patients_Name");

            builder.HasIndex(x => x.PhoneNumber)
                .HasDatabaseName("IX_Patients_PhoneNumber");

            builder.HasIndex(x => x.IdentityCard)
                .IsUnique()
                .HasFilter("\"IsSuspended\" = false AND \"IsCancelled\" = false")
                .HasDatabaseName("IX_Patients_IdentityCard");

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);

            // Table Comment
            builder.ToTable(t => t.HasComment("Bảng thông tin bệnh nhân"));
        }
    }
}