using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRequestForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestDetails_RequestForms_RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropTable(
                name: "RequestForms",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequestDetails_RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropColumn(
                name: "RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.AddColumn<int>(
                name: "ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Mã tiếp nhận");

            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                comment: "Số phiếu yêu cầu");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestDetails_ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "ReceptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestDetails_Receptions_ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "ReceptionId",
                principalSchema: "public",
                principalTable: "Receptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestDetails_Receptions_ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequestDetails_ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropColumn(
                name: "ReceptionId",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                schema: "public",
                table: "ServiceRequestDetails");

            migrationBuilder.AddColumn<int>(
                name: "RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Mã phiếu yêu cầu");

            migrationBuilder.CreateTable(
                name: "RequestForms",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceptionId = table.Column<int>(type: "integer", nullable: false, comment: "Mã tiếp nhận"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày tạo phiếu"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo phiếu"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng"),
                    RequestNumber = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, comment: "Số phiếu yêu cầu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestForms_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalSchema: "public",
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bảng phiếu yêu cầu dịch vụ");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestDetails_RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "RequestFormId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_ReceptionId",
                schema: "public",
                table: "RequestForms",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_RequestNumber",
                schema: "public",
                table: "RequestForms",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestDetails_RequestForms_RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "RequestFormId",
                principalSchema: "public",
                principalTable: "RequestForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
