using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VaccinationReception.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "DiseaseGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm bệnh"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Mô tả nhóm bệnh"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseGroups", x => x.Id);
                },
                comment: "Bảng nhóm bệnh");

            migrationBuilder.CreateTable(
                name: "ServiceGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên nhóm dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceGroups", x => x.Id);
                },
                comment: "Bảng nhóm dịch vụ");

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã dịch vụ"),
                    ServiceName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên dịch vụ"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, comment: "Đơn giá"),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false, comment: "Mã phòng ban"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                },
                comment: "Bảng dịch vụ");

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã loại dịch vụ"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Tên loại dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                },
                comment: "Loại hình dịch vụ tiếp nhận");

            migrationBuilder.CreateTable(
                name: "DiseaseGroupServices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiseaseGroupId = table.Column<int>(type: "integer", nullable: false, comment: "Mã nhóm bệnh"),
                    ServiceId = table.Column<int>(type: "integer", nullable: false, comment: "Mã dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseGroupServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseGroupServices_DiseaseGroups_DiseaseGroupId",
                        column: x => x.DiseaseGroupId,
                        principalSchema: "public",
                        principalTable: "DiseaseGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiseaseGroupServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng liên kết nhóm bệnh và dịch vụ");

            migrationBuilder.CreateTable(
                name: "ServiceGroupServices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceGroupId = table.Column<int>(type: "integer", nullable: false, comment: "Mã nhóm dịch vụ"),
                    ServiceId = table.Column<int>(type: "integer", nullable: false, comment: "Mã dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceGroupServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceGroupServices_ServiceGroups_ServiceGroupId",
                        column: x => x.ServiceGroupId,
                        principalSchema: "public",
                        principalTable: "ServiceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceGroupServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng liên kết nhóm dịch vụ và dịch vụ");

            migrationBuilder.CreateTable(
                name: "Receptions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false, comment: "Mã bệnh nhân"),
                    ReceptionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Ngày tiếp nhận"),
                    ServiceTypeId = table.Column<int>(type: "integer", nullable: false, comment: "Loại dịch vụ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receptions_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng tiếp nhận bệnh nhân");

            migrationBuilder.CreateTable(
                name: "ReceptionVaccinations",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceptionId = table.Column<int>(type: "integer", nullable: false, comment: "Mã tiếp nhận"),
                    VaccineId = table.Column<int>(type: "integer", nullable: false, comment: "Mã vắc xin"),
                    Quantity = table.Column<int>(type: "integer", nullable: false, comment: "Số lượng"),
                    IsReadyToUse = table.Column<bool>(type: "boolean", nullable: false, comment: "Sẵn sàng sử dụng"),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Ngày dự kiến tiêm"),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Ngày xuất hóa đơn"),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Ngày hẹn tiêm"),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false, comment: "Đã thanh toán"),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false, comment: "Đã xác nhận"),
                    Note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Ghi chú"),
                    TestResultEntry = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Kết quả thử"),
                    DoctorId = table.Column<int>(type: "integer", nullable: false, comment: "Mã bác sĩ"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptionVaccinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceptionVaccinations_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalSchema: "public",
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bảng chỉ định tiêm chủng");

            migrationBuilder.CreateTable(
                name: "RequestForms",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceptionId = table.Column<int>(type: "integer", nullable: false, comment: "Mã tiếp nhận"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo phiếu"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo phiếu"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
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

            migrationBuilder.CreateTable(
                name: "ScreeningEvaluationReports",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Khóa chính")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentFullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Họ tên phụ huynh"),
                    ParentPhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Số điện thoại phụ huynh"),
                    WeightKg = table.Column<double>(type: "double precision", nullable: false, comment: "Cân nặng (kg)"),
                    BodyTemperatureC = table.Column<double>(type: "double precision", nullable: false, comment: "Nhiệt độ cơ thể (°C)"),
                    BloodPressureSystolic = table.Column<int>(type: "integer", nullable: false, comment: "Huyết áp tâm thu"),
                    BloodPressureDiastolic = table.Column<int>(type: "integer", nullable: false, comment: "Huyết áp tâm trương"),
                    HasSevereFeverAfterPreviousVaccination = table.Column<bool>(type: "boolean", nullable: false, comment: "Sốt nặng sau tiêm trước"),
                    HasAcuteOrChronicDisease = table.Column<bool>(type: "boolean", nullable: false, comment: "Bệnh cấp/mạn tính"),
                    IsOnOrRecentlyEndedCorticosteroids = table.Column<bool>(type: "boolean", nullable: false, comment: "Đang/đã dùng corticosteroid"),
                    HasAbnormalTemperatureOrVitals = table.Column<bool>(type: "boolean", nullable: false, comment: "Nhiệt độ/sinh hiệu bất thường"),
                    HasAbnormalHeartSound = table.Column<bool>(type: "boolean", nullable: false, comment: "Nghe tim bất thường"),
                    HasHeartValveDisorder = table.Column<bool>(type: "boolean", nullable: false, comment: "Rối loạn van tim"),
                    HasNeurologicalAbnormalities = table.Column<bool>(type: "boolean", nullable: false, comment: "Bất thường thần kinh"),
                    IsUnderweightBelow2000g = table.Column<bool>(type: "boolean", nullable: false, comment: "Thiếu cân < 2000g"),
                    HasOtherContraindications = table.Column<bool>(type: "boolean", nullable: false, comment: "Chống chỉ định khác"),
                    IsEligibleForVaccination = table.Column<bool>(type: "boolean", nullable: false, comment: "Đủ điều kiện tiêm"),
                    IsContraindicatedForVaccination = table.Column<bool>(type: "boolean", nullable: false, comment: "Chống chỉ định"),
                    IsVaccinationDeferred = table.Column<bool>(type: "boolean", nullable: false, comment: "Tạm hoãn"),
                    IsReferredToHospital = table.Column<bool>(type: "boolean", nullable: false, comment: "Chuyển viện"),
                    ReceptionId = table.Column<int>(type: "integer", nullable: false, comment: "Khóa ngoại đến bảng tiếp nhận"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningEvaluationReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreeningEvaluationReports_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalSchema: "public",
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bảng ghi nhận đánh giá sàng lọc trước tiêm");

            migrationBuilder.CreateTable(
                name: "ServiceRequestDetails",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestFormId = table.Column<int>(type: "integer", nullable: false, comment: "Mã phiếu yêu cầu"),
                    ServiceId = table.Column<int>(type: "integer", nullable: false, comment: "Mã dịch vụ"),
                    Quantity = table.Column<int>(type: "integer", nullable: false, comment: "Số lượng"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, comment: "Đơn giá"),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Ngày xuất hóa đơn"),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false, comment: "Đã thanh toán"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequestDetails_RequestForms_RequestFormId",
                        column: x => x.RequestFormId,
                        principalSchema: "public",
                        principalTable: "RequestForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequestDetails_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bảng chi tiết yêu cầu dịch vụ");

            migrationBuilder.InsertData(
                schema: "public",
                table: "DiseaseGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh có khả năng lây truyền từ người sang người", "Nhóm bệnh truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Các bệnh không có khả năng lây truyền", "Nhóm bệnh không truyền nhiễm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "GroupName", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ tiêm chủng cơ bản", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm dịch vụ tiêm chủng đặc biệt", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "Name" },
                values: new object[,]
                {
                    { 1, "VAC001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Tiêm chủng dịch vụ" },
                    { 2, "VAC002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Tiêm chủng đặc biệt" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DepartmentId", "LastUpdatedAt", "LastUpdatedBy", "ServiceCode", "ServiceName", "UnitPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "VAC001", "Tiêm vắc xin 5 trong 1", 500000m },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "VAC002", "Tiêm vắc xin 6 trong 1", 600000m }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "DiseaseGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DiseaseGroupId", "LastUpdatedAt", "LastUpdatedBy", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceGroupServices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LastUpdatedAt", "LastUpdatedBy", "ServiceGroupId", "ServiceId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroups_GroupName",
                schema: "public",
                table: "DiseaseGroups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_DiseaseGroupId",
                schema: "public",
                table: "DiseaseGroupServices",
                column: "DiseaseGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseGroupServices_ServiceId",
                schema: "public",
                table: "DiseaseGroupServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_PatientId",
                schema: "public",
                table: "Receptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_ReceptionDate",
                schema: "public",
                table: "Receptions",
                column: "ReceptionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_ServiceTypeId",
                schema: "public",
                table: "Receptions",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionVaccinations_ReceptionId",
                schema: "public",
                table: "ReceptionVaccinations",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionVaccinations_VaccineId",
                schema: "public",
                table: "ReceptionVaccinations",
                column: "VaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_ReceptionId",
                schema: "public",
                table: "RequestForms",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningEvaluationReports_ReceptionId",
                schema: "public",
                table: "ScreeningEvaluationReports",
                column: "ReceptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroups_GroupName",
                schema: "public",
                table: "ServiceGroups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceGroupId",
                schema: "public",
                table: "ServiceGroupServices",
                column: "ServiceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroupServices_ServiceId",
                schema: "public",
                table: "ServiceGroupServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestDetails_RequestFormId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "RequestFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestDetails_ServiceId",
                schema: "public",
                table: "ServiceRequestDetails",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DepartmentId",
                schema: "public",
                table: "Services",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCode",
                schema: "public",
                table: "Services",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceName",
                schema: "public",
                table: "Services",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_Code",
                schema: "public",
                table: "ServiceTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiseaseGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ReceptionVaccinations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ScreeningEvaluationReports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroupServices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceRequestDetails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DiseaseGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RequestForms",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Receptions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceTypes",
                schema: "public");
        }
    }
}
