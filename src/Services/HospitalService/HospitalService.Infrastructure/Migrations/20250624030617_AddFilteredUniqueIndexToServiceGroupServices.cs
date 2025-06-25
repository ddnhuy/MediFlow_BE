using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexToServiceGroupServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId_ServiceId",
                schema: "public",
                table: "ServiceGroupServices");

            migrationBuilder.DropIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "ServiceGroupId", "ServiceId" },
                unique: true,
                filter: "\"IsCancelled\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                columns: new[] { "DiseaseGroupId", "ServiceId" },
                unique: true,
                filter: "\"IsCancelled\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId_ServiceId",
                schema: "public",
                table: "ServiceGroupServices");

            migrationBuilder.DropIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "ServiceGroupId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                columns: new[] { "DiseaseGroupId", "ServiceId" },
                unique: true);
        }
    }
}
