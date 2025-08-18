using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CheckDuplicatedInforContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedDate",
                schema: "public",
                table: "Contracts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Ngày dự kiến tiêm theo kế hoạch",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Ngày dự kiến tiêm theo kế hoạch");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractCode_Active",
                schema: "public",
                table: "Contracts",
                column: "ContractCode",
                unique: true,
                filter: "\"IsSuspended\" = false AND \"IsCancelled\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractNumber_Active",
                schema: "public",
                table: "Contracts",
                column: "ContractNumber",
                unique: true,
                filter: "\"IsSuspended\" = false AND \"IsCancelled\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractCode_Active",
                schema: "public",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractNumber_Active",
                schema: "public",
                table: "Contracts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedDate",
                schema: "public",
                table: "Contracts",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Ngày dự kiến tiêm theo kế hoạch",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày dự kiến tiêm theo kế hoạch");
        }
    }
}
