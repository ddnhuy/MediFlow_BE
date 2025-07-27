using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSecondReceptionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "integer",
                nullable: true,
                comment: "Mã tiếp nhận phụ (nếu tiêm ở lần khác)");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionVaccinations_SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations",
                column: "SecondaryReceptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceptionVaccinations_Receptions_SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations",
                column: "SecondaryReceptionId",
                principalSchema: "public",
                principalTable: "Receptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceptionVaccinations_Receptions_SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.DropIndex(
                name: "IX_ReceptionVaccinations_SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.DropColumn(
                name: "SecondaryReceptionId",
                schema: "public",
                table: "ReceptionVaccinations");
        }
    }
}
