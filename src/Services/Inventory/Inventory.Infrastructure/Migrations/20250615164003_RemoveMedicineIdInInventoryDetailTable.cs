using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMedicineIdInInventoryDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicineId",
                table: "InventoryDetails");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineBatches_MedicineId",
                table: "MedicineBatches",
                column: "MedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineBatches_Medicines_MedicineId",
                table: "MedicineBatches",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineBatches_Medicines_MedicineId",
                table: "MedicineBatches");

            migrationBuilder.DropIndex(
                name: "IX_MedicineBatches_MedicineId",
                table: "MedicineBatches");

            migrationBuilder.AddColumn<int>(
                name: "MedicineId",
                table: "InventoryDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
