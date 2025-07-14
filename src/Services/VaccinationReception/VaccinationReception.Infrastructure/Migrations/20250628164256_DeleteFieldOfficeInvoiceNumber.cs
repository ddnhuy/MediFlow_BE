using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFieldOfficeInvoiceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficialInvoiceNumber",
                schema: "public",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                schema: "public",
                table: "Payments",
                type: "text",
                nullable: true,
                comment: "Số hóa đơn",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "Số hóa đơn tạm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                schema: "public",
                table: "Payments",
                type: "text",
                nullable: true,
                comment: "Số hóa đơn tạm",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "Số hóa đơn");

            migrationBuilder.AddColumn<string>(
                name: "OfficialInvoiceNumber",
                schema: "public",
                table: "Payments",
                type: "text",
                nullable: true,
                comment: "Số hóa đơn chính thức");
        }
    }
}
