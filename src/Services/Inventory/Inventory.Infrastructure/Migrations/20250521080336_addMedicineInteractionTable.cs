using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMedicineInteractionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicineInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedicineId1 = table.Column<int>(type: "integer", nullable: false),
                    MedicineId2 = table.Column<int>(type: "integer", nullable: false),
                    HarmfulEffects = table.Column<string>(type: "text", nullable: true),
                    Mechanism = table.Column<string>(type: "text", nullable: true),
                    PreventiveActions = table.Column<string>(type: "text", nullable: true),
                    ReferenceInfo = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineInteractions_Medicines_MedicineId1",
                        column: x => x.MedicineId1,
                        principalTable: "Medicines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicineInteractions_Medicines_MedicineId2",
                        column: x => x.MedicineId2,
                        principalTable: "Medicines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInteractions_MedicineId1",
                table: "MedicineInteractions",
                column: "MedicineId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInteractions_MedicineId2",
                table: "MedicineInteractions",
                column: "MedicineId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineInteractions");
        }
    }
}
