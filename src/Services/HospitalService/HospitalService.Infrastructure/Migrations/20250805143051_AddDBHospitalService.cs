using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDBHospitalService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "DiseaseGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm bệnh"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Mô tả nhóm bệnh"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
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
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
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
                    ServiceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã dịch vụ"),
                    ServiceName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên dịch vụ"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, comment: "Đơn giá"),
                    ExaminationService = table.Column<int>(type: "integer", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false, comment: "Mã phòng ban"),
                    ServiceType = table.Column<int>(type: "integer", nullable: true, comment: "Loại dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
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
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
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
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
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

            migrationBuilder.CreateTable(
                name: "ServiceTestParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ParameterName = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    StandardValue = table.Column<string>(type: "text", nullable: false),
                    EquipmentName = table.Column<string>(type: "text", nullable: true),
                    SpecimenType = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTestParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTestParameters_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "DiseaseGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh có khả năng lây truyền từ người sang người", "Nhóm bệnh truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh không có khả năng lây truyền", "Nhóm bệnh không truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm bệnh dùng để phân loại các dịch vụ khám sức khỏe nhằm đánh giá tình trạng người bệnh trước khi thực hiện tiêm chủng.", "Khám sàng lọc trước tiêm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Công khám", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Công tiêm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ xét nghiệm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "ExaminationService", "LastUpdatedAt", "LastUpdatedBy", "ServiceCode", "ServiceName", "ServiceType", "UnitPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "EXAMFEE", "Công khám", 0, 50000m },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "IM", "Công tiêm bắp (IM)", 1, 30000m },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "SC", "Công tiêm dưới da (SC)", 1, 25000m },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ID", "Công tiêm trong da (ID)", 1, 35000m },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "BLOOD001", "Xét nghiệm công thức máu", 2, 150000m },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HEPB001", "Xét nghiệm kháng thể viêm gan B", 2, 250000m }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "ServiceGroupId", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 4 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, 5 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, 6 }
                });

            migrationBuilder.InsertData(
                table: "ServiceTestParameters",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EquipmentName", "IsCancelled", "IsSuspended", "LastUpdatedAt", "LastUpdatedBy", "ParameterName", "ServiceId", "SpecimenType", "StandardValue", "Unit" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WBC (White Blood Cell)", 5, "Máu toàn phần", "4.0 - 11.0", "G/L" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "LYM (Lymphocyte)", 5, "Máu toàn phần", "20.0 - 40.0", "%" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "NEU (Neutrophil)", 5, "Máu toàn phần", "50.0 - 70.0", "%" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MON (Monocyte)", 5, "Máu toàn phần", "2.0 - 8.0", "%" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "EOS (Eosinophils)", 5, "Máu toàn phần", "1.0 - 4.0", "%" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "BASO (Basophils)", 5, "Máu toàn phần", "0.0 - 1.0", "%" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "RBC (Red Blood Cell)", 5, "Máu toàn phần", "4.0 - 5.5", "T/L" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HGB (Hemoglobin)", 5, "Máu toàn phần", "130 - 175", "g/L" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HCT (Hematocrit)", 5, "Máu toàn phần", "40.0 - 50.0", "%" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCV (Mean Corpuscular Volume)", 5, "Máu toàn phần", "80.0 - 100.0", "fL" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCH (Mean Corpuscular Hemoglobin)", 5, "Máu toàn phần", "27.0 - 32.0", "pg" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCHC (Mean Corpuscular Hemoglobin Concentration)", 5, "Máu toàn phần", "320 - 360", "g/L" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "RDW (Red Cell Distribution Width)", 5, "Máu toàn phần", "11.5 - 14.5", "%" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PLT (Platelet Count)", 5, "Máu toàn phần", "150 - 450", "G/L" },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PCT (Plateletcrit)", 5, "Máu toàn phần", "0.1 - 0.3", "%" },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PDW (Platelet Distribution Width)", 5, "Máu toàn phần", "10.0 - 17.0", "%" },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MPV (Mean Platelet Volume)", 5, "Máu toàn phần", "7.0 - 11.0", "fL" },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "P-LCR (Plateletcrit Larger Cell Ratio)", 5, "Máu toàn phần", "15.0 - 35.0", "%" },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBsAb (Anti-HBs)", 6, "Huyết thanh", "> 10", "mIU/mL" },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBsAg", 6, "Huyết thanh", "< 0.05", "IU/mL" },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBeAg", 6, "Huyết thanh", "< 1.0", "S/CO" },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBe", 6, "Huyết thanh", "> 1.0", "S/CO" },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBc IgM", 6, "Huyết thanh", "< 1.0", "S/CO" },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBc IgG", 6, "Huyết thanh", "> 1.0", "S/CO" },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy PCR real-time", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBV-DNA", 6, "Huyết thanh", "< 20", "IU/mL" },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "AST (SGOT)", 6, "Huyết thanh", "7 - 40", "U/L" },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ALT (SGPT)", 6, "Huyết thanh", "7 - 40", "U/L" },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "GGT", 6, "Huyết thanh", "7 - 32", "U/L" },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Bilirubin toàn phần", 6, "Huyết thanh", "0.3 - 1.2", "mg/dL" },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Albumin", 6, "Huyết thanh", "3.5 - 5.0", "g/dL" }
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
                name: "IX_DiseaseGroupServices_DiseaseGroupId_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                columns: new[] { "DiseaseGroupId", "ServiceId" },
                unique: true,
                filter: "\"IsCancelled\" = false");

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
                name: "IX_ServiceGroupServices_ServiceGroupId_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "ServiceGroupId", "ServiceId" },
                unique: true,
                filter: "\"IsCancelled\" = false");

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

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTestParameters_ServiceId",
                table: "ServiceTestParameters",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiseaseGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceTestParameters");

            migrationBuilder.DropTable(
                name: "DiseaseGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "public");
        }
    }
}
