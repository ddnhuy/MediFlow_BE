using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbHospitalFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "",
                comment: "Trạng thái thanh toán");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "",
                comment: "Trạng thái thanh toán");

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceptionId = table.Column<int>(type: "integer", nullable: false, comment: "Mã tiếp nhận"),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Tổng số tiền"),
                    Method = table.Column<string>(type: "text", nullable: false, comment: "Phương thức thanh toán"),
                    Note = table.Column<string>(type: "text", nullable: true, comment: "Ghi chú"),
                    ATMTransactionCode = table.Column<string>(type: "text", nullable: true, comment: "Mã giao dịch ATM"),
                    PaymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Loại thanh toán"),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: true, comment: "Số hóa đơn tạm"),
                    OfficialInvoiceNumber = table.Column<string>(type: "text", nullable: true, comment: "Số hóa đơn chính thức"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Trạng thái thanh toán"),
                    OriginalPaymentId = table.Column<int>(type: "integer", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Payments_OriginalPaymentId",
                        column: x => x.OriginalPaymentId,
                        principalSchema: "public",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalSchema: "public",
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bảng thanh toán");

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<int>(type: "integer", nullable: false, comment: "Mã thanh toán"),
                    ReceptionVaccinationId = table.Column<int>(type: "integer", nullable: true, comment: "Mã tiêm chủng"),
                    ServiceRequestDetailId = table.Column<int>(type: "integer", nullable: true, comment: "Mã chi tiết yêu cầu dịch vụ"),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Số tiền"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "public",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_ReceptionVaccinations_ReceptionVaccinationId",
                        column: x => x.ReceptionVaccinationId,
                        principalSchema: "public",
                        principalTable: "ReceptionVaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_ServiceRequestDetails_ServiceRequestDetailId",
                        column: x => x.ServiceRequestDetailId,
                        principalSchema: "public",
                        principalTable: "ServiceRequestDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Chi tiết thanh toán");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_PaymentId",
                schema: "public",
                table: "PaymentDetails",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_ReceptionVaccinationId",
                schema: "public",
                table: "PaymentDetails",
                column: "ReceptionVaccinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_ServiceRequestDetailId",
                schema: "public",
                table: "PaymentDetails",
                column: "ServiceRequestDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OriginalPaymentId",
                schema: "public",
                table: "Payments",
                column: "OriginalPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReceptionId",
                schema: "public",
                table: "Payments",
                column: "ReceptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Đã thanh toán");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Đã thanh toán");
        }
    }
}
