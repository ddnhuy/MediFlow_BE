using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestNumberInRequestForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                schema: "public",
                table: "RequestForms",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                comment: "Số phiếu yêu cầu");

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_RequestNumber",
                schema: "public",
                table: "RequestForms",
                column: "RequestNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestForms_RequestNumber",
                schema: "public",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                schema: "public",
                table: "RequestForms");
        }
    }
}
