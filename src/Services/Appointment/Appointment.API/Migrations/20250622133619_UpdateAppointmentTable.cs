using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "PatientCode",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientDOB",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PatientFullName",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VaccineName",
                table: "Appointments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientCode",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientDOB",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientFullName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VaccineName",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
