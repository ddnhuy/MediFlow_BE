using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceTypes",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceTypes",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "RequestForms",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "RequestForms",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo phiếu",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo phiếu");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày dự kiến tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Ngày dự kiến tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày hẹn tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Ngày hẹn tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "PaymentDetails",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "PaymentDetails",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceTypes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceTypes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "RequestForms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "RequestForms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo phiếu",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo phiếu");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Ngày dự kiến tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày dự kiến tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Ngày xuất hóa đơn",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày xuất hóa đơn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Ngày hẹn tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày hẹn tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "PaymentDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "PaymentDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
