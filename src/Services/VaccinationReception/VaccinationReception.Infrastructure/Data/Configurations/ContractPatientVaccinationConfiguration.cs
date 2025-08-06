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
    public class ContractPatientVaccinationConfiguration : IEntityTypeConfiguration<ContractPatientVaccination>
    {
        public void Configure(EntityTypeBuilder<ContractPatientVaccination> builder)
        {
            // Table and Schema
            builder.ToTable("ContractPatientVaccinations", schema: "public");
            builder.ToTable(t => t.HasComment("Kế hoạch tiêm chủng của bệnh nhân theo hợp đồng")); // Table comment

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
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.ContractId)
                .IsRequired()
                .HasComment("Mã hợp đồng");

            builder.Property(x => x.PatientId)
                .IsRequired()
                .HasComment("Mã bệnh nhân");

            builder.Property(x => x.VaccineId)
                .IsRequired()
                .HasComment("Mã vắc xin");

            builder.Property(x => x.DoseNumber)
                .IsRequired()
                .HasComment("Liều số mấy");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("integer")
                .HasComment("Trạng thái của mũi tiêm kế hoạch");

            builder.Property(x => x.ReceptionVaccinationId)
                .IsRequired(false)
                .HasComment("Mã tiêm chủng thực tế");

            builder.HasOne(x => x.Contract)
                .WithMany(c => c.PlannedPatientVaccinations)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReceptionVaccination)
                .WithOne()
                .HasForeignKey<ContractPatientVaccination>(x => x.ReceptionVaccinationId) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Global Query Filter
            builder.HasQueryFilter(x => !x.IsCancelled);
        }
    }
}
