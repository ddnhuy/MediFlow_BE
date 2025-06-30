using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbRecepAndService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "integer",
                nullable: true,
                comment: "Mã bác sĩ",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã bác sĩ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Mã bác sĩ",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "Mã bác sĩ");
        }
    }
}
