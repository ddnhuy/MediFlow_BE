using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInfo.Grpc.Migrations
{
    /// <inheritdoc />
    public partial class CheckDuplicatedIdentityCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_IdentityCard",
                schema: "public",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentityCard",
                schema: "public",
                table: "Patients",
                column: "IdentityCard",
                unique: true,
                filter: "\"IsSuspended\" = false AND \"IsCancelled\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_IdentityCard",
                schema: "public",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentityCard",
                schema: "public",
                table: "Patients",
                column: "IdentityCard");
        }
    }
}
