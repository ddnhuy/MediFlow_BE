using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInfo.Grpc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DOB",
                schema: "public",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày sinh",
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "Ngày sinh");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "District", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, "TP. Thủ Đức", null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 1, null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AddressDetail", "CreatedBy", "District", "Email", "LastUpdatedAt", "LastUpdatedBy", "Province", "Ward" },
                values: new object[] { "369 Đường Trần Phú", 1, "Quận Phú Nhuận", null, new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 1, "TP. Huế", "Phường Vĩnh Ninh" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                schema: "public",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày cập nhật bản ghi cuối cùng",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày cập nhật bản ghi cuối cùng");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DOB",
                schema: "public",
                table: "Patients",
                type: "date",
                nullable: false,
                comment: "Ngày sinh",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày sinh");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Ngày tạo bản ghi",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Ngày tạo bản ghi");

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.01@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.02@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "District", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "Phường Linh Trung", "testpatient.03@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.04@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.05@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.07@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "Email", "LastUpdatedAt", "LastUpdatedBy" },
                values: new object[] { 0, "testpatient.08@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AddressDetail", "CreatedBy", "District", "Email", "LastUpdatedAt", "LastUpdatedBy", "Province", "Ward" },
                values: new object[] { "258 Đường Nguyễn Đình Chiểu", 0, "Phường Tân Hiệp", "testpatient.09@gmail.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "TP. Biên Hòa", "Phường Tân Hiệp" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Email", "Gender", "IdentityCard", "IsForeigner", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 9, "369 Đường Lý Tự Trọng", "BN009", new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(1991, 11, 7, 17, 0, 0, 0, DateTimeKind.Utc), "TP. Huế", "testpatient.10@gmail.com", (short)0, "901234567", true, 0, "Sarah Johnson", "0987654329", "TP. Huế", "Phường Phú Hội" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Patients",
                columns: new[] { "Id", "AddressDetail", "Code", "CreatedAt", "CreatedBy", "DOB", "District", "Email", "Gender", "IdentityCard", "IsPregnant", "LastUpdatedBy", "Name", "PhoneNumber", "Province", "Ward" },
                values: new object[] { 10, "741 Đường Đồng Khởi", "BN010", new DateTime(2023, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(1994, 2, 27, 17, 0, 0, 0, DateTimeKind.Utc), "TP. Nha Trang", "testpatient.11@gmail.com", (short)0, "012345678", true, 0, "Lê Thị Mai", "0987654330", "TP. Nha Trang", "Phường Vĩnh Hòa" });
        }
    }
}
