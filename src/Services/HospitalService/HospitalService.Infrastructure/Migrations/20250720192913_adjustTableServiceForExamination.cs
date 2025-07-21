using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustTableServiceForExamination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExaminationService",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: true);

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
                table: "ServiceGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Công khám", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Công tiêm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ xét nghiệm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "ExaminationService",
                value: null);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "ExaminationService",
                value: null);

            migrationBuilder.InsertData(
                schema: "public",
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "ExaminationService", "LastUpdatedAt", "LastUpdatedBy", "ServiceCode", "ServiceName", "UnitPrice" },
                values: new object[,]
                {
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "EXAMFEE", "Công khám", 50000m },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "IM", "Công tiêm bắp (IM)", 30000m },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "SC", "Công tiêm dưới da (SC)", 25000m },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ID", "Công tiêm trong da (ID)", 35000m },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "BLOOD001", "Xét nghiệm công thức máu", 150000m },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HEPB001", "Xét nghiệm kháng thể viêm gan B", 250000m }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "ServiceGroupId", "ServiceId" },
                values: new object[,]
                {
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, 4 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, 5 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, 6 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, 7 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, 8 }
                });

            migrationBuilder.InsertData(
                table: "ServiceTestParameters",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EquipmentName", "IsCancelled", "IsSuspended", "LastUpdatedAt", "LastUpdatedBy", "ParameterName", "ServiceId", "SpecimenType", "StandardValue", "Unit" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WBC (White Blood Cell)", 7, "Máu toàn phần", "4.0 - 11.0", "G/L" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "LYM (Lymphocyte)", 7, "Máu toàn phần", "20.0 - 40.0", "%" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "NEU (Neutrophil)", 7, "Máu toàn phần", "50.0 - 70.0", "%" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MON (Monocyte)", 7, "Máu toàn phần", "2.0 - 8.0", "%" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "EOS (Eosinophils)", 7, "Máu toàn phần", "1.0 - 4.0", "%" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "BASO (Basophils)", 7, "Máu toàn phần", "0.0 - 1.0", "%" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "RBC (Red Blood Cell)", 7, "Máu toàn phần", "4.0 - 5.5", "T/L" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HGB (Hemoglobin)", 7, "Máu toàn phần", "130 - 175", "g/L" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HCT (Hematocrit)", 7, "Máu toàn phần", "40.0 - 50.0", "%" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCV (Mean Corpuscular Volume)", 7, "Máu toàn phần", "80.0 - 100.0", "fL" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCH (Mean Corpuscular Hemoglobin)", 7, "Máu toàn phần", "27.0 - 32.0", "pg" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MCHC (Mean Corpuscular Hemoglobin Concentration)", 7, "Máu toàn phần", "320 - 360", "g/L" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "RDW (Red Cell Distribution Width)", 7, "Máu toàn phần", "11.5 - 14.5", "%" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PLT (Platelet Count)", 7, "Máu toàn phần", "150 - 450", "G/L" },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PCT (Plateletcrit)", 7, "Máu toàn phần", "0.1 - 0.3", "%" },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "PDW (Platelet Distribution Width)", 7, "Máu toàn phần", "10.0 - 17.0", "%" },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MPV (Mean Platelet Volume)", 7, "Máu toàn phần", "7.0 - 11.0", "fL" },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy phân tích huyết học tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "P-LCR (Plateletcrit Larger Cell Ratio)", 7, "Máu toàn phần", "15.0 - 35.0", "%" },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBsAb (Anti-HBs)", 8, "Huyết thanh", "> 10", "mIU/mL" },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBsAg", 8, "Huyết thanh", "< 0.05", "IU/mL" },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBeAg", 8, "Huyết thanh", "< 1.0", "S/CO" },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBe", 8, "Huyết thanh", "> 1.0", "S/CO" },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBc IgM", 8, "Huyết thanh", "< 1.0", "S/CO" },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy ELISA tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Anti-HBc IgG", 8, "Huyết thanh", "> 1.0", "S/CO" },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy PCR real-time", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HBV-DNA", 8, "Huyết thanh", "< 20", "IU/mL" },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "AST (SGOT)", 8, "Huyết thanh", "7 - 40", "U/L" },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ALT (SGPT)", 8, "Huyết thanh", "7 - 40", "U/L" },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "GGT", 8, "Huyết thanh", "7 - 32", "U/L" },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Bilirubin toàn phần", 8, "Huyết thanh", "0.3 - 1.2", "mg/dL" },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Máy sinh hóa tự động", false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Albumin", 8, "Huyết thanh", "3.5 - 5.0", "g/dL" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTestParameters_ServiceId",
                table: "ServiceTestParameters",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTestParameters");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroupServices",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroups",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceGroups",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Services",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "ExaminationService",
                schema: "public",
                table: "Services");
        }
    }
}
