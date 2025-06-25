using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTypeDateReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceptionDate",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tiếp nhận",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Ngày tiếp nhận");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceptionDate",
                schema: "public",
                table: "Receptions",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Ngày tiếp nhận",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tiếp nhận");
        }
    }
}
