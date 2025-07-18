using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.API.Migrations
{
    /// <inheritdoc />
    public partial class AdjustAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dose",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VaccineId",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Dose",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VaccineId",
                table: "Appointments");
        }
    }
}
