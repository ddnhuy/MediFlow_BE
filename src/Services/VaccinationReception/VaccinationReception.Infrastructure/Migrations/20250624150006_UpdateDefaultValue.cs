using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "NotPaid",
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "NotPaid",
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending",
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                schema: "public",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Receipt",
                comment: "Loại thanh toán",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "Loại thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                schema: "public",
                table: "Payments",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "Cash",
                comment: "Phương thức thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldComment: "Phương thức thanh toán");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ServiceRequestDetails",
                type: "varchar(20)",
                nullable: false,
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldDefaultValue: "NotPaid",
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "varchar(20)",
                nullable: false,
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldDefaultValue: "NotPaid",
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "Trạng thái thanh toán",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "Pending",
                oldComment: "Trạng thái thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                schema: "public",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "Loại thanh toán",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Receipt",
                oldComment: "Loại thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                schema: "public",
                table: "Payments",
                type: "varchar(20)",
                nullable: false,
                comment: "Phương thức thanh toán",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldDefaultValue: "Cash",
                oldComment: "Phương thức thanh toán");
        }
    }
}
