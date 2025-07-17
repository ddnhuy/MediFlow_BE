using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustTableReceptionVaccinationAndVaccinationReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.AddColumn<int>(
                name: "DoseNumber",
                table: "Vaccinations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Vaccinations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DoseNumber",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoseNumber",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "DoseNumber",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Đã xác nhận");
        }
    }
}
