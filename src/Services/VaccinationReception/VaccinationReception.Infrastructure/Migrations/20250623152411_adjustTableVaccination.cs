using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustTableVaccination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorName",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "ScheduleVaccinationDate",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "VaccinationConfirmation",
                table: "Vaccinations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VaccinationDate",
                table: "Vaccinations",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "VaccinationDate",
                table: "Vaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "Vaccinations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleVaccinationDate",
                table: "Vaccinations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaccinationConfirmation",
                table: "Vaccinations",
                type: "text",
                nullable: true);
        }
    }
}
