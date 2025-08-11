using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldScreeningEvaluationReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAbnormalCry",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Khóc bất thường");

            migrationBuilder.AddColumn<bool>(
                name: "HasImmunodeficiencyOrSuspectedHiv",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Suy giảm miễn dịch hoặc nghi ngờ HIV");

            migrationBuilder.AddColumn<bool>(
                name: "HasPaleSkinOrLips",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Da hoặc môi nhợt nhạt");

            migrationBuilder.AddColumn<bool>(
                name: "HasPoorFeeding",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Bú kém");

            migrationBuilder.AddColumn<bool>(
                name: "IsPretermBelow34Weeks",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Sinh non < 34 tuần");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAbnormalCry",
                schema: "public",
                table: "ScreeningEvaluationReports");

            migrationBuilder.DropColumn(
                name: "HasImmunodeficiencyOrSuspectedHiv",
                schema: "public",
                table: "ScreeningEvaluationReports");

            migrationBuilder.DropColumn(
                name: "HasPaleSkinOrLips",
                schema: "public",
                table: "ScreeningEvaluationReports");

            migrationBuilder.DropColumn(
                name: "HasPoorFeeding",
                schema: "public",
                table: "ScreeningEvaluationReports");

            migrationBuilder.DropColumn(
                name: "IsPretermBelow34Weeks",
                schema: "public",
                table: "ScreeningEvaluationReports");
        }
    }
}
