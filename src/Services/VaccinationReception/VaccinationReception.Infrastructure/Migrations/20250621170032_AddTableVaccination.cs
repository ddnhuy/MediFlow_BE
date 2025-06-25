using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableVaccination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VaccinationTestDate",
                schema: "public",
                table: "ReceptionVaccinations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    ReceptionVaccinationId = table.Column<int>(type: "integer", nullable: false),
                    MedicineBatchId = table.Column<int>(type: "integer", nullable: false),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    MedicineId = table.Column<int>(type: "integer", nullable: false),
                    MedicineName = table.Column<string>(type: "text", nullable: true),
                    VaccinationConfirmation = table.Column<string>(type: "text", nullable: true),
                    VaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduleVaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    DoctorId = table.Column<int>(type: "integer", nullable: false),
                    DoctorName = table.Column<string>(type: "text", nullable: true),
                    ObservationConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    HasReaction = table.Column<bool>(type: "boolean", nullable: false),
                    ReactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostVaccinationResult = table.Column<string>(type: "text", nullable: true),
                    PostVaccinationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasFeverAbove39 = table.Column<bool>(type: "boolean", nullable: false),
                    HasInjectionSiteReaction = table.Column<bool>(type: "boolean", nullable: false),
                    HasOtherReaction = table.Column<bool>(type: "boolean", nullable: false),
                    OtherReactionDescription = table.Column<string>(type: "text", nullable: true),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vaccinations_ReceptionVaccinations_ReceptionVaccinationId",
                        column: x => x.ReceptionVaccinationId,
                        principalSchema: "public",
                        principalTable: "ReceptionVaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_ReceptionVaccinationId",
                table: "Vaccinations",
                column: "ReceptionVaccinationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "VaccinationTestDate",
                schema: "public",
                table: "ReceptionVaccinations");
        }
    }
}
