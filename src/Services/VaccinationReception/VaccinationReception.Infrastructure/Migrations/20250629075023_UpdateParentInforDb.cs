using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParentInforDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParentPhoneNumber",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                comment: "Số điện thoại phụ huynh",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldComment: "Số điện thoại phụ huynh");

            migrationBuilder.AlterColumn<string>(
                name: "ParentFullName",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "Họ tên phụ huynh",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Họ tên phụ huynh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParentPhoneNumber",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                comment: "Số điện thoại phụ huynh",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Số điện thoại phụ huynh");

            migrationBuilder.AlterColumn<string>(
                name: "ParentFullName",
                schema: "public",
                table: "ScreeningEvaluationReports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "Họ tên phụ huynh",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Họ tên phụ huynh");
        }
    }
}
