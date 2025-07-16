using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFieldIsVaccinationTodayConfirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVaccinationTodayConfirmed",
                schema: "public",
                table: "Receptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVaccinationTodayConfirmed",
                schema: "public",
                table: "Receptions");
        }
    }
}
