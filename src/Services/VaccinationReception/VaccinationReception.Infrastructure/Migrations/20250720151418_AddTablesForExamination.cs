using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesForExamination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: true),
                    ReceptionId = table.Column<int>(type: "integer", nullable: true),
                    RequestNumber = table.Column<string>(type: "text", nullable: true),
                    PatientId = table.Column<int>(type: "integer", nullable: true),
                    Diagnose = table.Column<string>(type: "text", nullable: true),
                    ReceptionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PerformTechnicianId = table.Column<int>(type: "integer", nullable: true),
                    PerformTechnicianName = table.Column<string>(type: "text", nullable: true),
                    ReturnTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SampleType = table.Column<int>(type: "integer", nullable: true),
                    SampleQuality = table.Column<int>(type: "integer", nullable: true),
                    DoctorId = table.Column<int>(type: "integer", nullable: true),
                    DoctorName = table.Column<string>(type: "text", nullable: true),
                    Conclusion = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examinations_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalSchema: "public",
                        principalTable: "Receptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExaminationTestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExaminationId = table.Column<int>(type: "integer", nullable: true),
                    ResultValue = table.Column<string>(type: "text", nullable: true),
                    StandardValue = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationTestResults_Examinations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_ReceptionId",
                table: "Examinations",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationTestResults_ExaminationId",
                table: "ExaminationTestResults",
                column: "ExaminationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminationTestResults");

            migrationBuilder.DropTable(
                name: "Examinations");
        }
    }
}
