using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTableServiceTestParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                column: "ExaminationService",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                column: "ExaminationService",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                column: "ExaminationService",
                value: null);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                column: "ExaminationService",
                value: null);
        }
    }
}
