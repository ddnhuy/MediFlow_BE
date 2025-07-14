using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestDetails_Services_ServiceId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropTable(
                name: "DiseaseGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DiseaseGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "public");

            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                comment: "Số phiếu yêu cầu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestNumber",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.CreateTable(
                name: "DiseaseGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Mô tả nhóm bệnh"),
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm bệnh"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseGroups", x => x.Id);
                },
                comment: "Bảng nhóm bệnh");

            migrationBuilder.CreateTable(
                name: "ServiceGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm dịch vụ"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceGroups", x => x.Id);
                },
                comment: "Bảng nhóm dịch vụ");

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false, comment: "Mã phòng ban"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng"),
                    ServiceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã dịch vụ"),
                    ServiceName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên dịch vụ"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, comment: "Đơn giá")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                },
                comment: "Bảng dịch vụ");

            migrationBuilder.CreateTable(
                name: "DiseaseGroupServices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiseaseGroupId = table.Column<int>(type: "integer", nullable: false, comment: "Mã nhóm bệnh"),
                    ServiceId = table.Column<int>(type: "integer", nullable: false, comment: "Mã dịch vụ"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseGroupServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseGroupServices_DiseaseGroups_DiseaseGroupId",
                        column: x => x.DiseaseGroupId,
                        principalSchema: "public",
                        principalTable: "DiseaseGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiseaseGroupServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng liên kết nhóm bệnh và dịch vụ");

            migrationBuilder.CreateTable(
                name: "ServiceGroupServices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceGroupId = table.Column<int>(type: "integer", nullable: false, comment: "Mã nhóm dịch vụ"),
                    ServiceId = table.Column<int>(type: "integer", nullable: false, comment: "Mã dịch vụ"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceGroupServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceGroupServices_ServiceGroups_ServiceGroupId",
                        column: x => x.ServiceGroupId,
                        principalSchema: "public",
                        principalTable: "ServiceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceGroupServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng liên kết nhóm dịch vụ và dịch vụ");

            migrationBuilder.InsertData(
                schema: "public",
                table: "DiseaseGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh có khả năng lây truyền từ người sang người", "Nhóm bệnh truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh không có khả năng lây truyền", "Nhóm bệnh không truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ tiêm chủng cơ bản", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ tiêm chủng đặc biệt", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LastUpdatedAt", "LastUpdatedBy", "ServiceCode", "ServiceName", "UnitPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "VAC001", "Tiêm vắc xin 5 trong 1", 500000m },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "VAC002", "Tiêm vắc xin 6 trong 1", 600000m }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "DiseaseGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DiseaseGroupId", "LastUpdatedAt", "LastUpdatedBy", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "ServiceGroupId", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroups_GroupName",
                schema: "public",
                table: "DiseaseGroups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId",
                schema: "public",
                table: "DiseaseGroupServices",
                column: "DiseaseGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroups_GroupName",
                schema: "public",
                table: "ServiceGroups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId",
                schema: "public",
                table: "ServiceGroupServices",
                column: "ServiceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                column: "ServiceId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestDetails_Services_ServiceId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
