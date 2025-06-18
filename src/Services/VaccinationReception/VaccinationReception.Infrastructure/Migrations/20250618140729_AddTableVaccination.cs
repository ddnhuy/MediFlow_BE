using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableVaccination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiseaseGroupServices_Services_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceGroupServices_Services_ServiceId",
                schema: "public",
                table: "ServiceGroupServices");

            migrationBuilder.DropIndex(
                name: "IX_Services_DepartmentId",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCode",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceName",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_ServiceGroups_GroupName",
                schema: "public",
                table: "ServiceGroups");

            migrationBuilder.DropIndex(
                name: "IX_DiseaseGroups_GroupName",
                schema: "public",
                table: "DiseaseGroups");

            migrationBuilder.RenameTable(
                name: "Services",
                schema: "public",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "ServiceGroupServices",
                schema: "public",
                newName: "ServiceGroupServices");

            migrationBuilder.RenameTable(
                name: "ServiceGroups",
                schema: "public",
                newName: "ServiceGroups");

            migrationBuilder.RenameTable(
                name: "DiseaseGroupServices",
                schema: "public",
                newName: "DiseaseGroupServices");

            migrationBuilder.RenameTable(
                name: "DiseaseGroups",
                schema: "public",
                newName: "DiseaseGroups");

            migrationBuilder.AlterTable(
                name: "Services",
                oldComment: "Bảng dịch vụ");

            migrationBuilder.AlterTable(
                name: "ServiceGroupServices",
                oldComment: "Bảng liên kết nhóm dịch vụ và dịch vụ");

            migrationBuilder.AlterTable(
                name: "ServiceGroups",
                oldComment: "Bảng nhóm dịch vụ");

            migrationBuilder.AlterTable(
                name: "DiseaseGroupServices",
                oldComment: "Bảng liên kết nhóm bệnh và dịch vụ");

            migrationBuilder.AlterTable(
                name: "DiseaseGroups",
                oldComment: "Bảng nhóm bệnh");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "Services",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComment: "Đơn giá");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldComment: "Tên dịch vụ");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                table: "Services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "Mã dịch vụ");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                table: "Services",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái tạm ngưng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "Services",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái hủy");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã phòng ban");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Primary key")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã dịch vụ");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceGroupId",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã nhóm dịch vụ");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "ServiceGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                table: "ServiceGroupServices",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái tạm ngưng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "ServiceGroupServices",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái hủy");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Primary key")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "ServiceGroups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                table: "ServiceGroups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái tạm ngưng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "ServiceGroups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái hủy");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "ServiceGroups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldComment: "Tên nhóm dịch vụ");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceGroups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Primary key")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã dịch vụ");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "DiseaseGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                table: "DiseaseGroupServices",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái tạm ngưng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "DiseaseGroupServices",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái hủy");

            migrationBuilder.AlterColumn<int>(
                name: "DiseaseGroupId",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Mã nhóm bệnh");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DiseaseGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Primary key")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                table: "DiseaseGroups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái tạm ngưng");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "DiseaseGroups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Trạng thái hủy");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "DiseaseGroups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldComment: "Tên nhóm bệnh");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DiseaseGroups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Mô tả nhóm bệnh");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Người tạo bản ghi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Primary key")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    ReceptionVaccinationId = table.Column<int>(type: "integer", nullable: false),
                    MedicineBatchId = table.Column<int>(type: "integer", nullable: false),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    MedicineId = table.Column<int>(type: "integer", nullable: false),
                    MedicineName = table.Column<string>(type: "text", nullable: true),
                    VaccinationConfirmation = table.Column<string>(type: "text", nullable: true),
                    ScheduleVaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    DoctorId = table.Column<int>(type: "integer", nullable: false),
                    DoctorName = table.Column<string>(type: "text", nullable: true),
                    ExaminationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExaminationResult = table.Column<string>(type: "text", nullable: true),
                    VaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ObservationConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    HasReaction = table.Column<bool>(type: "boolean", nullable: false),
                    ReactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostVaccinationResult = table.Column<string>(type: "text", nullable: true),
                    PostVaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasFeverAbove39 = table.Column<bool>(type: "boolean", nullable: false),
                    HasInjectionSiteReaction = table.Column<bool>(type: "boolean", nullable: false),
                    HasOtherReaction = table.Column<bool>(type: "boolean", nullable: false),
                    OtherReactionDescription = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vaccinations_ReceptionVaccinations_ReceptionVaccinationId",
                        column: x => x.ReceptionVaccinationId,
                        principalSchema: "public",
                        principalTable: "ReceptionVaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_ReceptionVaccinationId",
                table: "Vaccinations",
                column: "ReceptionVaccinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiseaseGroupServices_Services_ServiceId",
                table: "DiseaseGroupServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceGroupServices_Services_ServiceId",
                table: "ServiceGroupServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiseaseGroupServices_Services_ServiceId",
                table: "DiseaseGroupServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceGroupServices_Services_ServiceId",
                table: "ServiceGroupServices");

            migrationBuilder.DropTable(
                name: "Vaccinations");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Services",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ServiceGroupServices",
                newName: "ServiceGroupServices",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ServiceGroups",
                newName: "ServiceGroups",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "DiseaseGroupServices",
                newName: "DiseaseGroupServices",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "DiseaseGroups",
                newName: "DiseaseGroups",
                newSchema: "public");

            migrationBuilder.AlterTable(
                name: "Services",
                schema: "public",
                comment: "Bảng dịch vụ");

            migrationBuilder.AlterTable(
                name: "ServiceGroupServices",
                schema: "public",
                comment: "Bảng liên kết nhóm dịch vụ và dịch vụ");

            migrationBuilder.AlterTable(
                name: "ServiceGroups",
                schema: "public",
                comment: "Bảng nhóm dịch vụ");

            migrationBuilder.AlterTable(
                name: "DiseaseGroupServices",
                schema: "public",
                comment: "Bảng liên kết nhóm bệnh và dịch vụ");

            migrationBuilder.AlterTable(
                name: "DiseaseGroups",
                schema: "public",
                comment: "Bảng nhóm bệnh");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                schema: "public",
                table: "Services",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                comment: "Đơn giá",
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                schema: "public",
                table: "Services",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                comment: "Tên dịch vụ",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                schema: "public",
                table: "Services",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "Mã dịch vụ",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                comment: "Người cập nhật bản ghi cuối cùng",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                schema: "public",
                table: "Services",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái tạm ngưng",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                schema: "public",
                table: "Services",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái hủy",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                comment: "Mã phòng ban",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                comment: "Người tạo bản ghi",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                comment: "Primary key",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                comment: "Mã dịch vụ",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceGroupId",
                schema: "public",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                comment: "Mã nhóm dịch vụ",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                schema: "public",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                comment: "Người cập nhật bản ghi cuối cùng",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                schema: "public",
                table: "ServiceGroupServices",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái tạm ngưng",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                schema: "public",
                table: "ServiceGroupServices",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái hủy",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "public",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                comment: "Người tạo bản ghi",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "ServiceGroupServices",
                type: "integer",
                nullable: false,
                comment: "Primary key",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                schema: "public",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                comment: "Người cập nhật bản ghi cuối cùng",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "ServiceGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                schema: "public",
                table: "ServiceGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái tạm ngưng",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                schema: "public",
                table: "ServiceGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái hủy",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                schema: "public",
                table: "ServiceGroups",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                comment: "Tên nhóm dịch vụ",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "public",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                comment: "Người tạo bản ghi",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "ServiceGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "ServiceGroups",
                type: "integer",
                nullable: false,
                comment: "Primary key",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                comment: "Mã dịch vụ",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                comment: "Người cập nhật bản ghi cuối cùng",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái tạm ngưng",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái hủy",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "DiseaseGroupId",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                comment: "Mã nhóm bệnh",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                comment: "Người tạo bản ghi",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "DiseaseGroupServices",
                type: "integer",
                nullable: false,
                comment: "Primary key",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "LastUpdatedBy",
                schema: "public",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                comment: "Người cập nhật bản ghi cuối cùng",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuspended",
                schema: "public",
                table: "DiseaseGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái tạm ngưng",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                schema: "public",
                table: "DiseaseGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Trạng thái hủy",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                schema: "public",
                table: "DiseaseGroups",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                comment: "Tên nhóm bệnh",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "DiseaseGroups",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "Mô tả nhóm bệnh",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "public",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                comment: "Người tạo bản ghi",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "DiseaseGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "DiseaseGroups",
                type: "integer",
                nullable: false,
                comment: "Primary key",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Services_DepartmentId",
                schema: "public",
                table: "Services",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCode",
                schema: "public",
                table: "Services",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceName",
                schema: "public",
                table: "Services",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroups_GroupName",
                schema: "public",
                table: "ServiceGroups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroups_GroupName",
                schema: "public",
                table: "DiseaseGroups",
                column: "GroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_DiseaseGroupServices_Services_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceGroupServices_Services_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
