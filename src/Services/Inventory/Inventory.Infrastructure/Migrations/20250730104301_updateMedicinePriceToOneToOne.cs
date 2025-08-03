using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateMedicinePriceToOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MedicinePrices_MedicineId",
                table: "MedicinePrices");

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePrices_MedicineId",
                table: "MedicinePrices",
                column: "MedicineId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MedicinePrices_MedicineId",
                table: "MedicinePrices");

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePrices_MedicineId",
                table: "MedicinePrices",
                column: "MedicineId");
        }
    }
}
