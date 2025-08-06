using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerInfo.Grpc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746501", "nguyen.van.an@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746502", "tran.thi.binh@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746503", "le.van.cuong@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746504", "pham.thi.dung@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746505", "john.smith@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746506", "hoang.van.minh@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746507", "nguyen.thi.huong@gmail.com" });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "Email" },
                values: new object[] { "CDCDN25032214264746508", "tran.van.phuc@gmail.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN001", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN002", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN003", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN004", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN005", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN006", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN007", null });

            migrationBuilder.UpdateData(
                schema: "public",
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "Email" },
                values: new object[] { "BN008", null });
        }
    }
}
