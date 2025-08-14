using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceptionForClosingWithIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasIssue",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssueNote",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasIssue",
                schema: "public",
                table: "Receptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                schema: "public",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssueNote",
                schema: "public",
                table: "Receptions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasIssue",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.DropColumn(
                name: "IssueNote",
                schema: "public",
                table: "ReceptionVaccinations");

            migrationBuilder.DropColumn(
                name: "HasIssue",
                schema: "public",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                schema: "public",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "IssueNote",
                schema: "public",
                table: "Receptions");
        }
    }
}
