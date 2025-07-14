using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Services",
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
                table: "Services",
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
                table: "ServiceGroupServices",
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
                table: "ServiceGroupServices",
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
                table: "ServiceGroups",
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
                table: "ServiceGroups",
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
                table: "DiseaseGroupServices",
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
                table: "DiseaseGroupServices",
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
                table: "DiseaseGroups",
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
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Services",
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
                table: "Services",
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
                table: "ServiceGroupServices",
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
                table: "ServiceGroupServices",
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
                table: "ServiceGroups",
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
                table: "ServiceGroups",
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
                table: "DiseaseGroupServices",
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
                table: "DiseaseGroupServices",
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
                table: "DiseaseGroups",
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
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");
        }
    }
}
