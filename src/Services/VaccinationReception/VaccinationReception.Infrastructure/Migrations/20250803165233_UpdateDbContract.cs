using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                schema: "public",
                table: "Receptions",
                type: "integer",
                nullable: true,
                comment: "Mã hợp đồng");

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã hợp đồng"),
                    ContractNumber = table.Column<int>(type: "integer", nullable: false, comment: "Số hợp đồng"),
                    ContractName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "Tên hợp đồng"),
                    CompanyName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "Tên công ty ký kết"),
                    UnitName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "Tên đơn vị trực thuộc công ty"),
                    Status = table.Column<int>(type: "integer", nullable: false, comment: "Trạng thái hợp đồng"),
                    ExpectedPatientCount = table.Column<int>(type: "integer", nullable: false, comment: "Số lượng bệnh nhân dự kiến"),
                    ExpectedVaccineCount = table.Column<int>(type: "integer", nullable: false, comment: "Số lượng vaccine dự kiến"),
                    ContractDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày ký hợp đồng"),
                    ExpectedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Ngày dự kiến tiêm theo kế hoạch"),
                    ContractValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Giá trị hợp đồng"),
                    AdvanceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true, comment: "Giá trị tạm ứng"),
                    ActualAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true, comment: "Giá trị thực tế"),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true, comment: "Diễn giải nội dung"),
                    FileContractId = table.Column<Guid>(type: "uuid", nullable: true, comment: "File hợp đồng id"),
                    FileVaccinationEnrollmentId = table.Column<Guid>(type: "uuid", nullable: true, comment: "File excel đăng ký vacicnation id"),
                    FileContractName = table.Column<string>(type: "text", nullable: true, comment: "File hợp đồng"),
                    FileVaccinationEnrollmentName = table.Column<string>(type: "text", nullable: true, comment: "File excel đăng ký vacicnation"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", maxLength: 256, nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                },
                comment: "Hợp đồng");

            migrationBuilder.CreateTable(
                name: "ContractPatientVaccinations",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false, comment: "Mã hợp đồng"),
                    PatientId = table.Column<int>(type: "integer", nullable: false, comment: "Mã bệnh nhân"),
                    VaccineId = table.Column<int>(type: "integer", nullable: false, comment: "Mã vắc xin"),
                    DoseNumber = table.Column<int>(type: "integer", nullable: false, comment: "Liều số mấy"),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, comment: "Trạng thái của mũi tiêm kế hoạch"),
                    ReceptionVaccinationId = table.Column<int>(type: "integer", nullable: true, comment: "Mã tiêm chủng thực tế"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", maxLength: 256, nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPatientVaccinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractPatientVaccinations_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractPatientVaccinations_ReceptionVaccinations_Reception~",
                        column: x => x.ReceptionVaccinationId,
                        principalSchema: "public",
                        principalTable: "ReceptionVaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Kế hoạch tiêm chủng của bệnh nhân theo hợp đồng");

            migrationBuilder.CreateTable(
                name: "ContractServiceDetails",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false, comment: "Mã hợp đồng"),
                    VaccineId = table.Column<int>(type: "integer", nullable: true, comment: "Mã vắc-xin"),
                    ServiceId = table.Column<int>(type: "integer", nullable: true, comment: "Mã dịch vụ"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Đơn giá của dịch vụ/vắc-xin này theo hợp đồng"),
                    Quantity = table.Column<int>(type: "integer", nullable: false, comment: "Số lượng thực tế"),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Tổng tiền thực tế cho mục này"),
                    ActualQuantity = table.Column<int>(type: "integer", nullable: true),
                    ActualTotalAmount = table.Column<int>(type: "integer", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractServiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractServiceDetails_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Chi tiết dịch vụ/vắc-xin trong hợp đồng");

            migrationBuilder.CreateTable(
                name: "PaymentContracts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<int>(type: "integer", nullable: false, comment: "Hop Dong Id"),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Số hóa đơn"),
                    VATInvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Số HĐ GTGT"),
                    InvoiceType = table.Column<int>(type: "integer", nullable: false, comment: "Loại hóa đơn"),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false, comment: "Người lập hóa đơn"),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Giá trị hợp đồng"),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false, comment: "Hình thức thanh toán"),
                    Status = table.Column<int>(type: "integer", nullable: true, comment: "Trạng thái thanh toán"),
                    TaxCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Mã số thuế đơn vị"),
                    OrganizationName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "Tên đơn vị thanh toán"),
                    ATMCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Mã giao dịch thẻ ATM"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentContracts_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "public",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Hợp đồng thanh toán");

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "Name" },
                values: new object[] { 3, "VAC003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Tiêm chủng hợp đồng" });

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_ContractId",
                schema: "public",
                table: "Receptions",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPatientVaccinations_ContractId",
                schema: "public",
                table: "ContractPatientVaccinations",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPatientVaccinations_ReceptionVaccinationId",
                schema: "public",
                table: "ContractPatientVaccinations",
                column: "ReceptionVaccinationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractServiceDetails_ContractId",
                schema: "public",
                table: "ContractServiceDetails",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentContracts_ContractId",
                schema: "public",
                table: "PaymentContracts",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Contracts_ContractId",
                schema: "public",
                table: "Receptions",
                column: "ContractId",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Contracts_ContractId",
                schema: "public",
                table: "Receptions");

            migrationBuilder.DropTable(
                name: "ContractPatientVaccinations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ContractServiceDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PaymentContracts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Receptions_ContractId",
                schema: "public",
                table: "Receptions");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "public",
                table: "Receptions");
        }
    }
}
