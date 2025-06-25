using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeMethodInPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Method",
                schema: "public",
                table: "Payments",
                type: "varchar(20)",
                nullable: false,
                comment: "Phương thức thanh toán",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "Phương thức thanh toán");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Method",
                schema: "public",
                table: "Payments",
                type: "text",
                nullable: false,
                comment: "Phương thức thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldComment: "Phương thức thanh toán");
        }
    }
}
