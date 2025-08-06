using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToRefreshTokenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "RefreshTokens");
        }
    }
}
