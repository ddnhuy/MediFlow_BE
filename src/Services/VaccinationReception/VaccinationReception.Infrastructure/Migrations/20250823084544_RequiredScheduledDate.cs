using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequiredScheduledDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Ngày dự kiến tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Ngày dự kiến tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Ngày hẹn tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày hẹn tiêm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Ngày dự kiến tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày dự kiến tiêm");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Ngày hẹn tiêm",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Ngày hẹn tiêm");
        }
    }
}
