using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerInfo.Grpc.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Mã bệnh nhân"),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Tên bệnh nhân"),
                    Gender = table.Column<short>(type: "smallint", nullable: false, comment: "Giới tính (0: Nữ, 1: Nam)"),
                    DOB = table.Column<DateTime>(type: "date", nullable: false, comment: "Ngày sinh"),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Số điện thoại"),
                    IdentityCard = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "CMND/CCCD"),
                    AddressDetail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, comment: "Địa chỉ chi tiết"),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "Tỉnh/Thành phố"),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "Quận/Huyện"),
                    Ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "Phường/Xã"),
                    IsPregnant = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Có thai hay không"),
                    IsForeigner = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Có phải người nước ngoài hay không"),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái tạm ngưng"),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Trạng thái hủy"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày tạo bản ghi"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người tạo bản ghi"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Ngày cập nhật bản ghi cuối cùng"),
                    LastUpdatedBy = table.Column<int>(type: "integer", nullable: false, comment: "Người cập nhật bản ghi cuối cùng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                },
                comment: "Bảng thông tin bệnh nhân");

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 1, "123 Đường Nguyễn Huệ", "BN001", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9499), 0, new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)1, "123456789", 0, "Nguyễn Văn An", "0987654321", "TP. Hồ Chí Minh", "Phường Bến Nghé" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "IsPregnant", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 2, "456 Đường Lê Lợi", "BN002", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9602), 0, new DateTime(1985, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)0, "234567890", true, 0, "Trần Thị Bình", "0987654322", "TP. Hồ Chí Minh", "Phường Bến Thành" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[,]
                {
                    { 3, "789 Đường Đồng Khởi", "BN003", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9604), 0, new DateTime(1995, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)1, "345678901", 0, "Lê Văn Cường", "0987654323", "TP. Hồ Chí Minh", "Phường Nguyễn Thái Bình" },
                    { 4, "321 Đường Nguyễn Du", "BN004", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9606), 0, new DateTime(1988, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)0, "456789012", 0, "Phạm Thị Dung", "0987654324", "TP. Hồ Chí Minh", "Phường Bến Nghé" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "IsForeigner", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 5, "654 Đường Lê Duẩn", "BN005", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9607), 0, new DateTime(1980, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)1, "567890123", true, 0, "John Smith", "0987654325", "TP. Hồ Chí Minh", "Phường Bến Thành" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 6, "987 Đường Pasteur", "BN006", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9611), 0, new DateTime(1992, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 3", (short)1, "678901234", 0, "Hoàng Văn Minh", "0987654326", "TP. Hồ Chí Minh", "Phường Võ Thị Sáu" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "IsPregnant", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 7, "147 Đường Võ Văn Tần", "BN007", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9612), 0, new DateTime(1993, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 3", (short)0, "789012345", true, 0, "Nguyễn Thị Hương", "0987654327", "TP. Hồ Chí Minh", "Phường 6" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 8, "258 Đường Nguyễn Đình Chiểu", "BN008", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9614), 0, new DateTime(1987, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 3", (short)1, "890123456", 0, "Trần Văn Phúc", "0987654328", "TP. Hồ Chí Minh", "Phường 5" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "IsForeigner", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 9, "369 Đường Lý Tự Trọng", "BN009", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9615), 0, new DateTime(1991, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)0, "901234567", true, 0, "Sarah Johnson", "0987654329", "TP. Hồ Chí Minh", "Phường Bến Thành" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Gender", "IdentityCard", "IsPregnant", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 10, "741 Đường Đồng Khởi", "BN010", new DateTime(2025, 5, 23, 14, 39, 20, 176, DateTimeKind.Utc).AddTicks(9617), 0, new DateTime(1994, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quận 1", (short)0, "012345678", true, 0, "Lê Thị Mai", "0987654330", "TP. Hồ Chí Minh", "Phường Nguyễn Thái Bình" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Code",
                schema: "public",
                table: "Patients",
                column: "Code",
                unique: true,
                filter: "\"IsSuspended\" = false AND \"IsCancelled\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentityCard",
                schema: "public",
                table: "Patients",
                column: "IdentityCard");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Name",
                schema: "public",
                table: "Patients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                schema: "public",
                table: "Patients",
                column: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients",
                schema: "public");
        }
    }
}
