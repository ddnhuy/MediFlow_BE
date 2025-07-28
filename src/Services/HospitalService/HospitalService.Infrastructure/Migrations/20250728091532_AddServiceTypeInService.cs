using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeInService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: true,
                comment: "Loại dịch vụ");

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceType",
                value: null);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "ServiceType",
                value: null);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "ServiceType",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "ServiceType",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "ServiceType",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                column: "ServiceType",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                column: "ServiceType",
                value: null);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                column: "ServiceType",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                schema: "public",
                table: "Services");
        }
    }
}
